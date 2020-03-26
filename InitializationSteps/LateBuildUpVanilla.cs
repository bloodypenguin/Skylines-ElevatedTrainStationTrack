﻿using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static class LateBuildUpVanilla
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            var smallWord = "";
            switch (prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).GroupBy(g => Math.Round(g.m_position)).Count())
            {
                case 1:
                    smallWord = "Small";
                    break;
                case 4:
                    smallWord = "Large";
                    break;
            }
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        if (prefab.name.Contains("Station"))
                        {
                            return;
                        }
                        var bridgeAI = prefab.GetComponent<MOMMetroTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            var narrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Vanilla_Narrow")}.Vanilla Narrow_Data");
                            bridgeAI.m_bridgePillarInfo = narrowInfo;
                            bridgeAI.m_bridgePillarOffset = -0.75f;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem()
                            {
                                NarrowInfo = narrowInfo,
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide Median")}.Modern Wide Median_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide")}.Modern Wide_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"Overground Metro Elevated Pillar 01"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide Median_NoCol")}.Modern Wide Median NoCol_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide_NoCol")}.Modern Wide NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Median_NoCol")}.Modern Median NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Vanilla_Narrow_NoCol")}.Vanilla Narrow NoCol_Data"),
                                HeightLimit = 0,
                                HeightOffset = 0
                            });
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        if (prefab.name.Contains("Station"))
                        {
                            return;
                        }
                        var bpBuildingInfo = PrefabCollection<BuildingInfo>.FindLoaded($"Overground Metro Bridge Pillar 01");
                        //var bpBuildingInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroBridgePillar{smallWord}NoCol")}.MetroBridgePillar{smallWord}NoCol_Data");
                        if (bpBuildingInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<MOMMetroTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = bpBuildingInfo;
                            bridgeAI.pillarType = PillarType.WideMedian;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 0, HeightOffset = 0, WideMedianInfo = bpBuildingInfo, WideMedianInfoNoCol = bpBuildingInfo });
                        }
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        if (prefab.name.Contains("Station"))
                        {
                            CustomizationSteps.SetupStationProps(prefab, version);
                        }
                        else
                        {
                            CustomizationSteps.SetupTrackProps(prefab, version);
                        }
                        break;
                    }
            }
        }

    }
}
