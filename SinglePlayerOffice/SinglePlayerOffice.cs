﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;

namespace SinglePlayerOffice {
    class SinglePlayerOffice : Script {

        public static ScriptSettings Configs { get; private set; }
        public static MenuPool MenuPool { get; private set; }
        public static bool IsHudHidden { get; set; }
        public static Arcadius Arcadius { get; private set; }
        public static LomBank LomBank { get; private set; }
        public static MazeBank MazeBank { get; private set; }
        public static MazeBankWest MazeBankWest { get; private set; }

        public SinglePlayerOffice() {
            Tick += OnTick;
            LoadConfigs();
            LoadMPMap();
            MenuPool = new MenuPool();
            IsHudHidden = false;
            Arcadius = new Arcadius();
            LomBank = new LomBank();
            MazeBank = new MazeBank();
            MazeBankWest = new MazeBankWest();
        }

        private static void LoadConfigs() {
            try {
                Configs = ScriptSettings.Load(@"scripts\SinglePlayerOffice.ini");
            }
            catch (Exception ex) {
                Logger.Log(ex.Message);
            }
        }

        private static void LoadMPMap() {
            Function.Call(Hash._LOAD_MP_DLC_MAPS);
            Function.Call(Hash._ENABLE_MP_DLC_MAPS, 1);
        }

        public static void DisplayHelpTextThisFrame(string text) {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        public static Building GetCurrentBuilding() {
            int currentInteriorID = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character);
            if (Game.Player.Character.Position.DistanceTo(Arcadius.Entrance.Trigger) < 20f || Arcadius.InteriorIDs.Contains(currentInteriorID) || Game.Player.Character.Position.DistanceTo(Arcadius.HeliPad.Trigger) < 20f) return Arcadius;
            else if (Game.Player.Character.Position.DistanceTo(LomBank.Entrance.Trigger) < 20f || LomBank.InteriorIDs.Contains(currentInteriorID) || Game.Player.Character.Position.DistanceTo(LomBank.HeliPad.Trigger) < 20f) return LomBank;
            else if (Game.Player.Character.Position.DistanceTo(MazeBank.Entrance.Trigger) < 20f || MazeBank.InteriorIDs.Contains(currentInteriorID) || Game.Player.Character.Position.DistanceTo(MazeBank.HeliPad.Trigger) < 20f) return MazeBank;
            else if (Game.Player.Character.Position.DistanceTo(MazeBankWest.Entrance.Trigger) < 20f || MazeBankWest.InteriorIDs.Contains(currentInteriorID) || Game.Player.Character.Position.DistanceTo(MazeBankWest.HeliPad.Trigger) < 20f) return MazeBankWest;
            return null;
        }

        private void OnTick(object sender, EventArgs e) {
            MenuPool.ProcessMenus();
            if (IsHudHidden) Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
            if (GetCurrentBuilding() != null) GetCurrentBuilding().OnTick();
        }

        protected override void Dispose(bool A_0) {
            if (A_0) {
                Arcadius.Dispose();
                LomBank.Dispose();
                MazeBank.Dispose();
                MazeBankWest.Dispose();
                World.RenderingCamera = null;
                World.DestroyAllCameras();
            }
        }

    }
}
