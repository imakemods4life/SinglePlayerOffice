﻿using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace SinglePlayerOffice.Interactions {

    internal class SofaAndTv : Interaction {

        private readonly List<string> idleAnims;
        private readonly Tv tv;

        private Prop remote;

        public SofaAndTv(Tv tv, Vector3 pos, Vector3 rot) {
            this.tv = tv;
            idleAnims = new List<string> { "idle_a", "idle_b", "idle_c" };
            Position = pos;
            Rotation = rot;
        }

        public override string HelpText => "Press ~INPUT_CONTEXT~ to sit on the couch";
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }

        public override void Update() {
            switch (State) {
                case 0:

                    if (!Game.Player.Character.IsDead && !Game.Player.Character.IsInVehicle() &&
                        Game.Player.Character.Position.DistanceTo(Position) < 1.5f &&
                        World.GetNearbyPeds(Position, 0.5f).Length == 0) {
                        Utilities.DisplayHelpTextThisFrame(HelpText);

                        if (Game.IsControlJustPressed(2, Control.Context)) {
                            Game.Player.Character.Weapons.Select(WeaponHash.Unarmed);
                            UI.IsHudHidden = true;
                            State = 1;
                        }
                    }

                    break;
                case 1:
                    Function.Call(Hash.REQUEST_ANIM_DICT, "anim@amb@office@seating@male@var_a@base@");
                    Function.Call(Hash.REQUEST_ANIM_DICT, "anim@amb@office@game@seated@male@var_c@base@");
                    if (Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "anim@amb@office@seating@male@var_a@base@") &&
                        Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "anim@amb@office@game@seated@male@var_c@base@"))
                        State = 2;

                    break;
                case 2:
                    initialPos = Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION,
                        "anim@amb@office@seating@male@var_a@base@", "enter", Position.X, Position.Y, Position.Z,
                        Rotation.X, Rotation.Y, Rotation.Z, 0, 2);
                    initialRot = Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_ROTATION,
                        "anim@amb@office@seating@male@var_a@base@", "enter", Position.X, Position.Y, Position.Z,
                        Rotation.X, Rotation.Y, Rotation.Z, 0, 2);
                    Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, Game.Player.Character, initialPos.X, initialPos.Y,
                        initialPos.Z, 1f, 1000, initialRot.Z, 0f);
                    State = 3;

                    break;
                case 3:

                    if (Function.Call<int>(Hash.GET_SCRIPT_TASK_STATUS, Game.Player.Character, 0x7d8f4411) == 1) break;

                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@seating@male@var_a@base@", "enter", 1.5f, -1.5f, 13, 16, 1.5f, 0);
                    State = 4;

                    break;
                case 4:

                    if (Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, syncSceneHandle) < 1f) break;

                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@seating@male@var_a@base@", "base", 4f, -1.5f, 13, 16, 1148846080, 0);
                    State = 5;

                    break;
                case 5:
                    Utilities.DisplayHelpTextThisFrame(tv.HelpText + "~n~Press ~INPUT_AIM~ to stand up");

                    if (Game.IsControlJustPressed(2, Control.Context)) {
                        State = 6;

                        break;
                    }

                    if (Game.IsControlJustPressed(2, Control.Aim)) {
                        State = 9;

                        break;
                    }

                    if (Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, syncSceneHandle) < 1f) break;

                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@seating@male@var_a@base@",
                        idleAnims[Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 3)], 4f, -1.5f, 13, 16,
                        1148846080, 0);
                    State = 4;

                    break;
                case 6:

                    foreach (var prop in World.GetNearbyProps(Game.Player.Character.Position, 10f)) {
                        if (prop.Model.Hash != 608950395 && prop.Model.Hash != 1036195894) continue;

                        tv.Prop = prop;

                        break;
                    }

                    var remoteModel = new Model("ex_prop_tv_settop_remote");
                    remoteModel.Request(250);

                    if (remoteModel.IsInCdImage && remoteModel.IsValid) {
                        while (!remoteModel.IsLoaded) Script.Wait(50);
                        remote = World.CreateProp(remoteModel, Vector3.Zero, false, false);
                    }

                    remoteModel.MarkAsNoLongerNeeded();
                    remote.AttachTo(Game.Player.Character, Game.Player.Character.GetBoneIndex(Bone.SKEL_R_Hand),
                        new Vector3(0.12f, 0.02f, -0.04f), new Vector3(-10f, 100f, 120f));
                    if (tv.IsTvOn && Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 2) == 1)
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH1, Game.Player.Character, "TV_BORED",
                            "SPEECH_PARAMS_FORCE");
                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@game@seated@male@var_c@base@", "enter_a", 4f, -4f, 13, 16, 1000f, 0);
                    State = 7;

                    break;
                case 7:

                    if (Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, syncSceneHandle) < 1f) break;

                    tv.State = 1;
                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@game@seated@male@var_c@base@", "exit_a", 4f, -4f, 13, 16, 1000f, 0);
                    State = 8;

                    break;
                case 8:

                    if (Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, syncSceneHandle) < 1f) break;

                    remote.Delete();
                    State = 4;

                    break;
                case 9:
                    syncSceneHandle = Function.Call<int>(Hash.CREATE_SYNCHRONIZED_SCENE, Position.X, Position.Y,
                        Position.Z, Rotation.X, Rotation.Y, Rotation.Z, 2);
                    Function.Call(Hash.TASK_SYNCHRONIZED_SCENE, Game.Player.Character, syncSceneHandle,
                        "anim@amb@office@seating@male@var_a@base@", "exit", 4f, -4f, 13, 16, 1000f, 0);
                    State = 10;

                    break;
                case 10:

                    if (Function.Call<float>(Hash.GET_SYNCHRONIZED_SCENE_PHASE, syncSceneHandle) < 1f) break;

                    UI.IsHudHidden = false;
                    Game.Player.Character.Task.ClearAll();
                    Function.Call(Hash.REMOVE_ANIM_DICT, "anim@amb@office@seating@male@var_a@base@");
                    Function.Call(Hash.REMOVE_ANIM_DICT, "anim@amb@office@game@seated@male@var_c@base@");
                    State = 0;

                    break;
            }
        }

        public override void Reset() {
            tv.Reset();
        }

        public override void Dispose() {
            tv.Dispose();
            remote?.Delete();
        }

    }

}