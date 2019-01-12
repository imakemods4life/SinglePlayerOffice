﻿using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using SinglePlayerOffice.Interactions;

namespace SinglePlayerOffice.Buildings {
    internal class Arcadius : Building {
        public Arcadius() {
            try {
                Name = "Arcadius Business Center";
                data = ScriptSettings.Load($@"scripts\SinglePlayerOffice\{Name}\data.ini");
                Description =
                    "The City within the City, the Arcadius Business Center boats more AAA hedge funds, smoothie bars and executive suicides per square foot of office space than any other building in the business district. Welcome to Cutting edge.";
                Price = 2250000;
                InteriorIDs = new List<int> {
                    134657, 237313, 237569, 237057, 236289, 236545, 236801, 237825, 238081, 238337, 253441, 253697,
                    253953, 254209
                };
                exteriorMapObjects = new List<string>
                    { "hei_dt1_02_w01", "dt1_02_helipad_01", "dt1_02_dt1_emissive_dt1_02" };
                Owner = (Owner) data.GetValue("Owner", "Owner", -1);
                Entrance = new Entrance {
                    TriggerPos = new Vector3(-118.791f, -608.376f, 36.281f),
                    SpawnPos = new Vector3(-117.505f, -608.885f, 36.281f),
                    SpawnHeading = 250.669f
                };
                GarageEntrance = new GarageEntrance {
                    TriggerPos = new Vector3(-143.998f, -576.076f, 32.060f),
                    SpawnPos = new Vector3(-143.998f, -576.076f, 32.060f),
                    SpawnHeading = 160f,
                    InteriorId = 134657,
                    VehicleElevatorEntrance = new VehicleElevatorEntrance(
                        new Vector3(-140.868f, -573.453f, 33.425f),
                        new Vector3(-10f, 0f, 125f),
                        60f)
                };
                Office = new Office {
                    TriggerPos = new Vector3(-141.670f, -620.949f, 168.821f),
                    SpawnPos = new Vector3(-140.327f, -622.087f, 168.820f),
                    SpawnHeading = 184.412f,
                    ExteriorIpLs = new List<string> { "hei_dt1_02_exshadowmesh" },
                    InteriorIDs =
                        new List<int> { 237313, 237569, 237057, 236289, 236545, 236801, 237825, 238081, 238337 },
                    PurchaseCamPos = new Vector3(-142.224f, -646.676f, 170f),
                    PurchaseCamRot = new Vector3(-5f, 0f, -43f),
                    PurchaseCamFov = 60f,
                    InteriorStyles = new List<InteriorStyle> {
                        new InteriorStyle("Executive Rich", 0, "ex_dt1_02_office_02b"),
                        new InteriorStyle("Executive Cool", 415000, "ex_dt1_02_office_02c"),
                        new InteriorStyle("Executive Contrast", 500000, "ex_dt1_02_office_02a"),
                        new InteriorStyle("Old Spice Classical", 685000, "ex_dt1_02_office_01b"),
                        new InteriorStyle("Old Spice Vintage", 760000, "ex_dt1_02_office_01c"),
                        new InteriorStyle("Old Spice Warms", 950000, "ex_dt1_02_office_01a"),
                        new InteriorStyle("Power Broker Conservative", 835000, "ex_dt1_02_office_03b"),
                        new InteriorStyle("Power Broker Polished", 910000, "ex_dt1_02_office_03c"),
                        new InteriorStyle("Power Broker Ice", 1000000, "ex_dt1_02_office_03a")
                    },
                    HasExtraDecors = data.GetValue("Interiors", "HasExtraOfficeDecors", false),
                    Boss = new Boss(new Vector3(-146.516f, -645.987f, 168.417f)),
                    Pa = new Pa(
                        new Vector3(-138.994f, -634.089f, 168.423f),
                        new Vector3(0f, 0f, -174f)),
                    Staffs =
                        new List<Staff> {
                            new Staff(PedHash.Business01AMM, new Vector3(-145.851f, -642.375f, 168.417f)),
                            new Staff(PedHash.Business02AMY, new Vector3(-147.866f, -643.760f, 168.417f)),
                            new Staff(PedHash.Business01AFY, new Vector3(-145.729f, -643.541f, 168.417f)),
                            new Staff(PedHash.Business04AFY, new Vector3(-147.991f, -642.567f, 168.417f))
                        },
                    Sofas = new List<Sofa> {
                        new Sofa(new Vector3(-136.210f, -640.910f, 167.83f), new Vector3(0f, 0f, -44f)),
                        new Sofa(new Vector3(-140.388f, -641.317f, 167.83f), new Vector3(0f, 0f, 50f)),
                        new Sofa(new Vector3(-148.038f, -629.249f, 167.83f), new Vector3(0f, 0f, 6f)),
                        new Sofa(new Vector3(-150.244f, -630.662f, 167.83f), new Vector3(0f, 0f, 97f)),
                        new Sofa(new Vector3(-150.090f, -632.143f, 167.83f), new Vector3(0f, 0f, 97f))
                    },
                    Wardrobe = new Wardrobe(new Vector3(-132.303f, -632.859f, 168.820f), new Vector3(0f, 0f, -84f))
                };
                Office.InteriorStyle =
                    GetOfficeInteriorStyle(data.GetValue("Interiors", "OfficeInteriorStyle", "Executive Rich"));
                Office.ExtraDecorsPrice = Office.HasExtraDecors ? 1650000 : 0;
                Office.SofaAndTv = new SofaAndTv(Office.Tv, new Vector3(-137.806f, -644.631f, 167.820f),
                    new Vector3(0f, 0f, -174f));
                GarageOne = new Garage {
                    TriggerPos = new Vector3(-198.649f, -580.730f, 136.001f),
                    SpawnPos = new Vector3(-196.790f, -580.510f, 136.001f),
                    SpawnHeading = -84f,
                    Ipl = "imp_dt1_02_cargarage_a",
                    ExteriorIpLs = new List<string> { "hei_dt1_02_impexpproxy_a", "hei_dt1_02_impexpemproxy_a" },
                    InteriorId = 253441,
                    PurchaseCamPos = new Vector3(-196.790f, -580.510f, 138f),
                    PurchaseCamRot = new Vector3(-5f, -1f, -84f),
                    PurchaseCamFov = 90f,
                    DecorationCamPos = new Vector3(-191.550f, -588.963f, 136.000f),
                    DecorationCamRot = new Vector3(5f, 0f, -34f),
                    DecorationCamFov = 60f,
                    DecorationStyle =
                        GetGarageDecorationStyle(data.GetValue("Interiors", "GarageOneDecorationStyle",
                            "Decoration 1")),
                    LightingCamPos = new Vector3(-192.656f, -585.665f, 136.000f),
                    LightingCamRot = new Vector3(55f, 0f, -135f),
                    LightingCamFov = 70f,
                    LightingStyle =
                        GetGarageLightingStyle(data.GetValue("Interiors", "GarageOneLightingStyle", "Lighting 1")),
                    NumberingCamPos = new Vector3(-191.463f, -573.653f, 136.001f),
                    NumberingCamRot = new Vector3(12f, 0f, 6.520f),
                    NumberingCamFov = 60f,
                    NumberingStyle =
                        GetGarageOneNumberingStyle(data.GetValue("Interiors", "GarageOneNumberingStyle", "Signage 1")),
                    Sofas = new List<Sofa> {
                        new Sofa(new Vector3(-192.444f, -587.386f, 135.02f), new Vector3(0f, 0f, 96f)),
                        new Sofa(new Vector3(-189.793f, -589.582f, 135.02f), new Vector3(0f, 0f, 187f))
                    },
                    VehicleElevator = new VehicleElevator(
                        new Vector3(0f, 0f, 96.096f),
                        new Vector3(-181.798f, -581.548f, 134.116f),
                        new Vector3(-181.798f, -581.548f, 139.466f),
                        new Vector3(-181.798f, -581.548f, 144.816f))
                };
                GarageTwo = new Garage {
                    TriggerPos = new Vector3(-124.515f, -571.676f, 136.001f),
                    SpawnPos = new Vector3(-122.979f, -571.062f, 136.001f),
                    SpawnHeading = -69f,
                    Ipl = "imp_dt1_02_cargarage_b",
                    ExteriorIpLs = new List<string> { "hei_dt1_02_impexpproxy_b", "hei_dt1_02_impexpemproxy_b" },
                    InteriorId = 253697,
                    PurchaseCamPos = new Vector3(-122.979f, -571.062f, 138f),
                    PurchaseCamRot = new Vector3(-5f, -1f, -69f),
                    PurchaseCamFov = 90f,
                    DecorationCamPos = new Vector3(-115.602f, -577.715f, 136.001f),
                    DecorationCamRot = new Vector3(5f, 0f, -19f),
                    DecorationCamFov = 60f,
                    DecorationStyle =
                        GetGarageDecorationStyle(data.GetValue("Interiors", "GarageTwoDecorationStyle",
                            "Decoration 1")),
                    LightingCamPos = new Vector3(-117.451f, -574.830f, 136.000f),
                    LightingCamRot = new Vector3(55f, 0f, -120f),
                    LightingCamFov = 70f,
                    LightingStyle =
                        GetGarageLightingStyle(data.GetValue("Interiors", "GarageTwoLightingStyle", "Lighting 1")),
                    NumberingCamPos = new Vector3(-119.268f, -563.180f, 136.000f),
                    NumberingCamRot = new Vector3(12f, 0f, 21f),
                    NumberingCamFov = 60f,
                    NumberingStyle =
                        GetGarageTwoNumberingStyle(data.GetValue("Interiors", "GarageTwoNumberingStyle", "Signage 1")),
                    Sofas = new List<Sofa> {
                        new Sofa(new Vector3(-116.744f, -576.420f, 135.02f), new Vector3(0f, 0f, 111f)),
                        new Sofa(new Vector3(-113.662f, -577.868f, 135.02f), new Vector3(0f, 0f, -159f))
                    },
                    VehicleElevator = new VehicleElevator(
                        new Vector3(0f, 0f, 110.928f),
                        new Vector3(-107.975f, -568.080f, 134.116f),
                        new Vector3(-107.975f, -568.080f, 139.466f),
                        new Vector3(-107.975f, -568.080f, 144.816f))
                };
                GarageThree = new Garage {
                    TriggerPos = new Vector3(-135.621f, -622.349f, 136.001f),
                    SpawnPos = new Vector3(-135.882f, -623.975f, 136.001f),
                    SpawnHeading = 171f,
                    Ipl = "imp_dt1_02_cargarage_c",
                    ExteriorIpLs = new List<string> { "hei_dt1_02_impexpproxy_c", "hei_dt1_02_impexpemproxy_c" },
                    InteriorId = 253953,
                    PurchaseCamPos = new Vector3(-135.882f, -623.975f, 138f),
                    PurchaseCamRot = new Vector3(-5f, -1f, 171f),
                    PurchaseCamFov = 90f,
                    DecorationCamPos = new Vector3(-145.349f, -627.036f, 136.001f),
                    DecorationCamRot = new Vector3(5f, 0f, -139f),
                    DecorationCamFov = 60f,
                    DecorationStyle =
                        GetGarageDecorationStyle(data.GetValue("Interiors", "GarageThreeDecorationStyle",
                            "Decoration 1")),
                    LightingCamPos = new Vector3(-141.992f, -626.889f, 136.000f),
                    LightingCamRot = new Vector3(55f, 0f, 120f),
                    LightingCamFov = 70f,
                    LightingStyle =
                        GetGarageLightingStyle(data.GetValue("Interiors", "GarageThreeLightingStyle", "Lighting 1")),
                    NumberingCamPos = new Vector3(-130.550f, -631.243f, 136.000f),
                    NumberingCamRot = new Vector3(12f, 0f, -98f),
                    NumberingCamFov = 60f,
                    NumberingStyle =
                        GetGarageThreeNumberingStyle(data.GetValue("Interiors", "GarageThreeNumberingStyle",
                            "Signage 1")),
                    Sofas = new List<Sofa> {
                        new Sofa(new Vector3(-143.683f, -626.663f, 135.02f), new Vector3(0f, 0f, -9f)),
                        new Sofa(new Vector3(-146.439f, -628.631f, 135.02f), new Vector3(0f, 0f, 83f))
                    },
                    VehicleElevator = new VehicleElevator(
                        new Vector3(0f, 0f, -9.082f),
                        new Vector3(-140.817f, -638.450f, 134.116f),
                        new Vector3(-140.817f, -638.450f, 139.466f),
                        new Vector3(-140.817f, -638.450f, 144.816f))
                };
                ModShop = new ModShop {
                    TriggerPos = new Vector3(-138.322f, -592.926f, 167.000f),
                    SpawnPos = new Vector3(-139.104f, -591.805f, 167.000f),
                    SpawnHeading = 34.856f,
                    Ipl = "imp_dt1_02_modgarage",
                    ExteriorIpLs = new List<string>
                        { "hei_dt1_02_impexpproxy_modshop", "hei_dt1_02_impexpemproxy_modshop" },
                    InteriorId = 254209,
                    PurchaseCamPos = new Vector3(-142.051f, -591.137f, 169f),
                    PurchaseCamRot = new Vector3(-20f, 0f, 130f),
                    PurchaseCamFov = 70f,
                    FloorStyle = GetModShopFloorStyle(data.GetValue("Interiors", "ModShopFloorStyle", "Floor 1"))
                };
                ModShop.SofaAndTv = new SofaAndTv(ModShop.Tv, new Vector3(-137.396f, -600.697f, 166.65f),
                    new Vector3(0f, 0f, -50f));
                HeliPad = new HeliPad {
                    TriggerPos = new Vector3(-155.139f, -602.231f, 201.735f),
                    SpawnPos = new Vector3(-156.460f, -603.294f, 201.735f),
                    SpawnHeading = 128.035f
                };
                PurchaseCamPos = new Vector3(-167.906f, -487.694f, 40f);
                PurchaseCamRot = new Vector3(30f, 0, -170f);
                PurchaseCamFov = 70f;

                CreateEntranceBlip();
                if (IsOwned)
                    CreateGarageEntranceBlip();
                CreatePurchaseMenu();
                CreateTeleportMenu();
                CreateGarageEntranceMenu();
                CreateVehicleElevatorMenu();
                CreatePaMenu();
            }
            catch (Exception ex) {
                Logger.Log(ex.ToString());
            }
        }
    }
}