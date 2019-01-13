﻿using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace SinglePlayerOffice.Interactions {

    internal class RadioStation {

        public RadioStation(string name, string des, string gameName) {
            Name = name;
            Description = des;
            GameName = gameName;
        }

        public string Name { get; }
        public string Description { get; }
        public string GameName { get; }

    }

    internal class Radio : Interaction {

        private Prop radio;

        static Radio() {
            Stations = new List<RadioStation> {
                new RadioStation("Los Santos Rock Radio", "Get on the good ship power pop and rock.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.LosSantosRockRadio)),
                new RadioStation("Non-Stop-Pop FM", "Pop hits from the 80s, 90s, noughties, and today.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.NonStopPopFM)),
                new RadioStation("Radio Los Santos", "Contemporary Hip Hop blazing into your stereo.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.RadioLosSantos)),
                new RadioStation("Channel X", "Real Punk. Real West Coast.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.ChannelX)),
                new RadioStation("West Coast Talk Radio",
                    "Real Talk. Real Issues. Real Patronizing. Radio that’s all about you.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.WestCoastTalkRadio)),
                new RadioStation("Rebel Radio", "The true sound of Blaine County - drunk, armed, and ready to party.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.RebelRadio)),
                new RadioStation("Soulwax FM", "A continuous mix of dance and electronica.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.SoulwaxFM)),
                new RadioStation("East Los FM",
                    "Mexican electronica mixing corridos and traditional songs with hip hop, rock, and ska.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.EastLosFM)),
                new RadioStation("West Coast Classics",
                    "Coming at you from the city of Davis - music from the days of the pager.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.WestCoastClassics)),
                new RadioStation("Blue Ark", "The hottest reggae, dancehall and dub served up by The Upsetter.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.TheBlueArk)),
                new RadioStation("WorldWide FM", "Join Gilles Peterson as he brings that perfect beat to Los Santos.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.WorldWideFM)),
                new RadioStation("FlyLo FM", "A mix to carry you through the Los Santos freeways at top speed.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.FlyloFM)),
                new RadioStation("The Lowdown 91.1", "The groove, the soul, the R&B. You dig?",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.TheLowdown)),
                new RadioStation("The Lab", "Dropping science with Dr. No and the Chemical Bro.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.TheLab)),
                new RadioStation("Radio Mirror Park", "Indie modern rock from the underground.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.RadioMirrorPark)),
                new RadioStation("Space 103.2", "Bringing the Funk to your Assdroid.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.Space)),
                new RadioStation("Vinewood Boulevard Radio",
                    "The soundtrack to your broken dreams and unspent potential.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.VinewoodBoulevardRadio)),
                new RadioStation("blonded Los Santos 97.8 FM", "An eclectic mix of savory soul, hip hop and deep cuts.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.BlondedLosSantos)),
                new RadioStation("Blaine County Radio", "The home of the patriot. Enough said.",
                    Function.Call<string>(Hash.GET_RADIO_STATION_NAME, (int) GTA.RadioStation.BlaineCountyRadio))
            };
        }

        public static List<RadioStation> Stations { get; }

        public override string HelpText => !IsRadioOn
            ? "Press ~INPUT_CONTEXT~ to turn on the radio"
            : "Press ~INPUT_CONTEXT~ to turn off the radio";

        public bool IsRadioOn { get; private set; }
        public RadioStation Station { get; set; }

        public override void Update() {
            var currentBuilding = SinglePlayerOffice.CurrentBuilding;

            switch (State) {
                case 0:
                    if (!Game.Player.Character.IsDead && !Game.Player.Character.IsInVehicle() &&
                        !UI.MenuPool.IsAnyMenuOpen())
                        foreach (var prop in World.GetNearbyProps(Game.Player.Character.Position, 1f))
                            switch (prop.Model.Hash) {
                                case -364924791:
                                case 2079380440:
                                    Utilities.DisplayHelpTextThisFrame(HelpText);

                                    if (Game.IsControlJustPressed(2, Control.Context)) {
                                        radio = prop;
                                        Game.Player.Character.Weapons.Select(WeaponHash.Unarmed);
                                        UI.IsHudHidden = true;
                                        State = 1;
                                    }

                                    break;
                            }

                    break;
                case 1:
                    Function.Call(Hash.REQUEST_ANIM_DICT, "anim@mp_radio@high_apment");
                    if (Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, "anim@mp_radio@high_apment"))
                        State = 2;

                    break;
                case 2:
                    initialPos = radio.GetOffsetInWorldCoords(new Vector3(0f, -0.65f, 0f));
                    initialRot = Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_ROTATION,
                        "anim@mp_radio@high_apment", "action_a_bedroom", radio.Position.X, radio.Position.Y,
                        radio.Position.Z, radio.Rotation.X, radio.Rotation.Y, radio.Rotation.Z, 0, 2);
                    Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, Game.Player.Character, initialPos.X, initialPos.Y,
                        initialPos.Z, 1f, 1, initialRot.Z, 0f);
                    State = 3;

                    break;
                case 3:

                    if (Function.Call<int>(Hash.GET_SCRIPT_TASK_STATUS, Game.Player.Character, 0x7d8f4411) == 1) break;

                    Game.Player.Character.Task.StandStill(-1);

                    if (!IsRadioOn && !UI.MenuPool.IsAnyMenuOpen()) {
                        UI.RadioMenu.Visible = true;
                        State = 0;
                    }

                    if (IsRadioOn)
                        State = 4;

                    break;
                case 4:
                    Game.Player.Character.Task.PlayAnimation("anim@mp_radio@high_apment", "action_a_bedroom");
                    State = 5;

                    break;
                case 5:

                    if (Function.Call<float>(Hash.GET_ENTITY_ANIM_CURRENT_TIME, Game.Player.Character,
                            "anim@mp_radio@high_apment", "action_a_bedroom") > 0.5f) {
                        if (!IsRadioOn) {
                            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED,
                                currentBuilding.CurrentLocation.RadioEmitter,
                                true);
                            Function.Call(Hash._0x0E0CD610D5EB6C85,
                                currentBuilding.CurrentLocation.RadioEmitter,
                                radio);
                            Function.Call(Hash.SET_EMITTER_RADIO_STATION,
                                currentBuilding.CurrentLocation.RadioEmitter,
                                Station.GameName);
                            IsRadioOn = true;
                        }
                        else {
                            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED,
                                currentBuilding.CurrentLocation.RadioEmitter,
                                false);
                            IsRadioOn = false;
                        }

                        State = 6;
                    }

                    break;
                case 6:

                    if (Function.Call<bool>(Hash.IS_ENTITY_PLAYING_ANIM, Game.Player.Character,
                        "anim@mp_radio@high_apment", "action_a_bedroom", 3)) break;

                    if (IsRadioOn && Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 2) == 1)
                        Function.Call(Hash._PLAY_AMBIENT_SPEECH1, Game.Player.Character, "RADIO_LIKE",
                            "SPEECH_PARAMS_FORCE");
                    UI.IsHudHidden = false;
                    Game.Player.Character.Task.ClearAll();
                    Function.Call(Hash.REMOVE_ANIM_DICT, "anim@mp_radio@high_apment");
                    State = 0;

                    break;
            }
        }

        public override void Reset() {
            IsRadioOn = false;
            Dispose();
        }

        public override void Dispose() {
            Function.Call(Hash.SET_STATIC_EMITTER_ENABLED,
                SinglePlayerOffice.CurrentBuilding.CurrentLocation.RadioEmitter,
                false);
        }

    }

}