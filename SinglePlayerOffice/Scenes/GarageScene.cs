﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;

namespace SinglePlayerOffice {
    class Elevator {

        public Prop Base { get; set; }
        public Prop Platform { get; set; }

    }
    class ElevatorGate {

        public Prop Prop { get; set; }
        public Vector3 InitialPos { get; set; }

        public ElevatorGate(Prop prop, Vector3 pos) {
            Prop = prop;
            InitialPos = pos;
        }

    }
    class GarageScene : Scene {

        private static VehicleInfo vehicleInfo;
        private static Scaleform vehicleInfoScaleform;

        private Elevator elevator;
        private List<ElevatorGate> elevatorGates;
        private List<VehicleInfo> vehicleInfoList;
        private bool isVehiclesCreated;

        public Vector3 ElevatorLevelAPos { get; set; }
        public Vector3 ElevatorLevelBPos { get; set; }
        public Vector3 ElevatorLevelCPos { get; set; }
        public Vector3 ElevatorPos { get; set; }
        public Vector3 ElevatorRot { get; set; }
        public int ElevatorStatus { get; set; }

        public GarageScene() {
            elevatorGates = new List<ElevatorGate>();
            vehicleInfoList = new List<VehicleInfo>();
            ActiveInteractions.AddRange(new List<Action> { ElevatorOnTick, VehicleInfoOnTick });
        }

        public void CreateElevator() {
            elevator = new Elevator();
            var model = new Model("imp_prop_ie_carelev01");
            model.Request(250);
            if (model.IsInCdImage && model.IsValid) {
                while (!model.IsLoaded) Script.Wait(50);
                elevator.Base = World.CreateProp(model, Vector3.Zero, Vector3.Zero, false, false);
            }
            model.MarkAsNoLongerNeeded();
            model = new Model("imp_prop_ie_carelev02");
            model.Request(250);
            if (model.IsInCdImage && model.IsValid) {
                while (!model.IsLoaded) Script.Wait(50);
                elevator.Platform = World.CreateProp(model, Vector3.Zero, Vector3.Zero, false, false);
                elevator.Platform.Position = ElevatorLevelAPos;
                elevator.Platform.Rotation = ElevatorRot;
                elevator.Platform.FreezePosition = true;
            }
            model.MarkAsNoLongerNeeded();
            elevator.Base.AttachTo(elevator.Platform, 0);
        }

        public void DeleteElevator() {
            if (elevator != null) {
                elevator.Base.Delete();
                elevator.Platform.Delete();
                elevator = null;
            }
        }

        private bool MoveElevator(Vector3 pos) {
            if (elevator.Platform.Position.DistanceTo(pos) > 0.01f) {
                if (elevator.Platform.Position.Z < pos.Z) elevator.Platform.Position = Vector3.Add(elevator.Platform.Position, new Vector3(0f, 0f, 0.005f));
                else elevator.Platform.Position = Vector3.Add(elevator.Platform.Position, new Vector3(0f, 0f, -0.005f));
                return true;
            }
            else return false;
        }

        private bool MoveElevatorGate(Prop gate, Vector3 pos) {
            if (gate.Position.DistanceTo(pos) > 0.01f) {
                if (gate.Position.Z < pos.Z) gate.Position = Vector3.Add(gate.Position, new Vector3(0f, 0f, 0.01f));
                else gate.Position = Vector3.Add(gate.Position, new Vector3(0f, 0f, -0.01f));
                return true;
            }
            else return false;
        }

        public Vector3 GetCurrentLevelElevatorPos() {
            if (Game.Player.Character.Position.Z > ElevatorLevelAPos.Z && Game.Player.Character.Position.Z < ElevatorLevelBPos.Z) return ElevatorLevelAPos;
            else if (Game.Player.Character.Position.Z > ElevatorLevelBPos.Z && Game.Player.Character.Position.Z < ElevatorLevelCPos.Z) return ElevatorLevelBPos;
            else if (Game.Player.Character.Position.Z > ElevatorLevelCPos.Z) return ElevatorLevelCPos;
            return Vector3.Zero;
        }

        public void AddVehicleInfo(Vehicle vehicle) {
            vehicle.IsPersistent = true;
            var info = new VehicleInfo(vehicle);
            vehicleInfoList.Add(info);
        }

        public void RemoveVehicleInfo(Vehicle vehicle) {
            foreach (var vehicleInfo in vehicleInfoList) {
                if (vehicleInfo.Vehicle == vehicle) {
                    vehicleInfoList.Remove(vehicleInfo);
                    break;
                }
            }
        }

        private string GetGarageFileName() {
            if (Location == Location.Building.GarageOne) return "GarageOne.xml";
            else if (Location == Location.Building.GarageTwo) return "GarageTwo.xml";
            else if (Location == Location.Building.GarageThree) return "GarageThree.xml";
            return null;
        }

        public void SaveVehicleInfoList() {
            foreach (var vehicleInfo in vehicleInfoList) {
                vehicleInfo.Position = vehicleInfo.Vehicle.Position;
                vehicleInfo.Rotation = vehicleInfo.Vehicle.Rotation;
            }
            var serializer = new XmlSerializer(typeof(List<VehicleInfo>));
            var fileName = GetGarageFileName();
            var writer = new StreamWriter(String.Format(@"scripts\SinglePlayerOffice\{0}\{1}", Location.Building.Name, fileName));
            serializer.Serialize(writer, vehicleInfoList);
            writer.Close();
        }

        public void LoadVehicleInfoList() {
            var serializer = new XmlSerializer(typeof(List<VehicleInfo>));
            var fileName = GetGarageFileName();
            var reader = new StreamReader(String.Format(@"scripts\SinglePlayerOffice\{0}\{1}", Location.Building.Name, fileName));
            vehicleInfoList = (List<VehicleInfo>)serializer.Deserialize(reader);
            reader.Close();
        }

        public void CreateVehicles() {
            LoadVehicleInfoList();
            foreach (var vehicleInfo in vehicleInfoList) {
                vehicleInfo.CreateVehicle();
            }
            isVehiclesCreated = true;
        }

        public void DeleteVehicles() {
            foreach (var vehicleInfo in vehicleInfoList) {
                vehicleInfo.DeleteVehicle();
            }
            isVehiclesCreated = false;
        }

        private void ElevatorOnTick() {
            switch (ElevatorStatus) {
                case 0:
                    if (!Game.Player.Character.IsDead && (Game.Player.Character.Position.DistanceTo(ElevatorLevelAPos) < 8f || Game.Player.Character.Position.DistanceTo(ElevatorLevelBPos) < 8f || Game.Player.Character.Position.DistanceTo(ElevatorLevelCPos) < 8f) && !SinglePlayerOffice.MenuPool.IsAnyMenuOpen()) {
                        SinglePlayerOffice.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to use the vehicle elevator");
                        if (Game.IsControlJustPressed(2, GTA.Control.Context)) {
                            ElevatorPos = GetCurrentLevelElevatorPos();
                            ElevatorStatus = 3;
                        }
                    }
                    break;
                case 1:
                    AddVehicleInfo(Game.Player.Character.CurrentVehicle);
                    elevator.Platform.Position = elevator.Platform.GetOffsetInWorldCoords(new Vector3(0f, 0f, -1f));
                    ElevatorStatus = 2;
                    break;
                case 2:
                    if (MoveElevator(ElevatorLevelAPos)) {
                        Game.Player.Character.CurrentVehicle.Position = elevator.Platform.GetOffsetInWorldCoords(new Vector3(0f, 0f, 1.3f));
                        Game.Player.Character.CurrentVehicle.Rotation = Vector3.Add(Game.Player.Character.CurrentVehicle.Rotation, new Vector3(0f, 0f, 0.2f));
                    }
                    else {
                        ElevatorPos = GetCurrentLevelElevatorPos();
                        SinglePlayerOffice.IsHudHidden = false;
                        Game.Player.Character.Task.ClearAll();
                        if (Location == Location.Building.GarageOne) Audio.PlaySoundFrontend("Speech_Floor_1", "DLC_IE_Garage_Elevator_Sounds");
                        else if (Location == Location.Building.GarageTwo) Audio.PlaySoundFrontend("Speech_Floor_2", "DLC_IE_Garage_Elevator_Sounds");
                        else if (Location == Location.Building.GarageThree) Audio.PlaySoundFrontend("Speech_Floor_3", "DLC_IE_Garage_Elevator_Sounds");
                        ElevatorStatus = 3;
                    }
                    break;
                case 3:
                    foreach (var gate in elevatorGates) MoveElevatorGate(gate.Prop, gate.InitialPos);
                    if (!MoveElevator(ElevatorPos)) {
                        var gates = new List<ElevatorGate>();
                        foreach (var prop in World.GetNearbyProps(elevator.Platform.Position, 4.5f)) {
                            if (prop.Model.Hash == -2088725999 || prop.Model.Hash == -1238206604 || (prop.Model.Hash == 1593297148 && elevator.Platform.Position.DistanceTo(ElevatorLevelAPos) < 1f)) {
                                gates.Add(new ElevatorGate(prop, prop.Position));
                                elevatorGates = gates;
                            }
                        }
                        ElevatorStatus = 4;
                    }
                    break;
                case 4:
                    if (!elevatorGates.TrueForAll(gate => MoveElevatorGate(gate.Prop, Vector3.Add(gate.InitialPos, new Vector3(0f, 0f, 3f))))) {
                        Location.Building.UpdateVehicleElevatorMenuButtons();
                        SinglePlayerOffice.IsHudHidden = true;
                        Location.Building.VehicleElevatorMenu.Visible = true;
                        ElevatorStatus = 0;
                    }
                    break;
            }
        }

        private void VehicleInfoOnTick() {
            var vehicle = World.GetClosestVehicle(Game.Player.Character.Position, 3f);
            if (!Game.Player.Character.IsDead && !Game.Player.Character.IsInVehicle() && vehicle != null) {
                if (vehicleInfo == null || vehicle != vehicleInfo.Vehicle) {
                    vehicleInfo = new VehicleInfo(vehicle);
                    if (vehicleInfoScaleform != null) {
                        vehicleInfoScaleform.Dispose();
                        Script.Wait(0);
                    }
                    vehicleInfoScaleform = new Scaleform("MP_CAR_STATS_01");
                    while (!vehicleInfoScaleform.IsLoaded) Script.Wait(0);
                    Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION, vehicleInfoScaleform.Handle, "SET_VEHICLE_INFOR_AND_STATS");
                    Function.Call(Hash._0x80338406F3475E55, "FM_TWO_STRINGS");
                    Function.Call(Hash._0xC63CD5D2920ACBE7, vehicleInfo.GetBrandName(true));
                    Function.Call(Hash._0xC63CD5D2920ACBE7, vehicleInfo.Vehicle.DisplayName);
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._0x80338406F3475E55, "MP_PROP_CAR0");
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._0xE83A3E3557A56640, vehicleInfo.GetLogoTextureDict());
                    Function.Call(Hash._0xE83A3E3557A56640, vehicleInfo.GetBrandName(false));
                    Function.Call(Hash._0x80338406F3475E55, "FMMC_VEHST_0");
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._0x80338406F3475E55, "FMMC_VEHST_1");
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._0x80338406F3475E55, "FMMC_VEHST_2");
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._0x80338406F3475E55, "FMMC_VEHST_3");
                    Function.Call(Hash._0x362E2D3FE93A9959);
                    Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, Function.Call<int>(Hash.ROUND, vehicleInfo.GetMaxSpeedInPercentage()));
                    Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, Function.Call<int>(Hash.ROUND, vehicleInfo.GetMaxAccelerationInPercentage()));
                    Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, Function.Call<int>(Hash.ROUND, vehicleInfo.GetMaxBrakingInPercentage()));
                    Function.Call(Hash._PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT, Function.Call<int>(Hash.ROUND, vehicleInfo.GetMaxTractionInPercentage()));
                    Function.Call(Hash._POP_SCALEFORM_MOVIE_FUNCTION_VOID);
                }
                Vector3 pos = vehicle.Position + new Vector3(0f, 0f, vehicle.Model.GetDimensions().Z + 2f);
                Vector3 camCoord = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
                Vector3 rot = new Vector3(0f, 0f, (180f - Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, camCoord.X - pos.X, camCoord.Y - pos.Y)));
                Vector3 scale = new Vector3(8f, 4.5f, 1f);
                vehicleInfoScaleform.Render3D(pos, rot, scale);
            }
            else vehicleInfo = null;
        }

        public override void OnTick() {
            if (Location == null) Location = SinglePlayerOffice.GetCurrentBuilding().GetCurrentLocation();
            if (elevator == null) {
                Location.Building.GarageOne.Scene.DeleteElevator();
                Location.Building.GarageTwo.Scene.DeleteElevator();
                Location.Building.GarageThree.Scene.DeleteElevator();
                CreateElevator();
            }
            if (!isVehiclesCreated) {
                Location.Building.GarageOne.Scene.DeleteVehicles();
                Location.Building.GarageTwo.Scene.DeleteVehicles();
                Location.Building.GarageThree.Scene.DeleteVehicles();
                CreateVehicles();
            }
            base.OnTick();
        }

        public override void Dispose() {
            if (vehicleInfoScaleform != null) vehicleInfoScaleform.Dispose();
            DeleteElevator();
            DeleteVehicles();
        }

    }
}