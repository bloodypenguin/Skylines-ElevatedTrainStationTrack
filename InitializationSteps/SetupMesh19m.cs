﻿using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;

namespace MetroOverhaul.InitializationSteps {
    partial class SetupMesh {
        public static void Setup19mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo) {
            var width = "";
            if (prefab.name.Contains("Dual Island")) {
                width = "31m";
            }
            //else if (prefab.name.Contains("Side Island"))
            //{
            //    width = "31_2m";
            //}
            else {
                width = "19m";
            }
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var traintrackInfo = Prefabs.Find<NetInfo>("Train Track");
            var railMaterial = traintrackInfo.m_segments[1].m_material;
            var railLODMaterial = traintrackInfo.m_segments[1].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            switch (version) {
                case NetInfoVersion.Ground: {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node4);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Pavement.obj",
                                $@"Meshes\{width}\Ground_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Station_Node_Pavement_LOD.obj");
                        node4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Node_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = elevatedMaterial;
                        node4.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated: {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[0].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var segment3 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node2);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Boosted_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Node_Pavement_LOD.obj");
                        node2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Node_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = railMaterial;
                        node2.m_lodMaterial = railLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
                case NetInfoVersion.Tunnel: {
                        var segment0 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment1 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment2 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment3 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment4 = metroStationInfo.m_segments[0].ShallowClone();
                        var node0 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node1 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node3 = metroStationInfo.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Station_Pavement_Gray_LOD.obj");
                        segment1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement.obj",
                            @"Meshes\10m\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Boosted_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Station_Pavement_Gray_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Node_Pavement.obj",
                            @"Meshes\10m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Node_ThirdRail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = railMaterial;
                        segment3.m_lodMaterial = railLODMaterial;
                        segment4.m_material = railMaterial;
                        segment4.m_lodMaterial = railLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node3.m_material = railMaterial;
                        node3.m_lodMaterial = railLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
            }
            //if (version == NetInfoVersion.Ground || version == NetInfoVersion.Elevated) {
            //    nodeList[0].m_flagsForbidden |= NetNode.Flags.End;
            //}
            for (int i = 0; i < nodeList.Count; i++) {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(prefab, version));
            prefab.m_nodes = nodeList.ToArray();
        }
    }
}
