﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using MetroOverhaul.NEXT.Extensions;
using UnityEngine;

namespace MetroOverhaul.NEXT
{
    public static partial class RoadHelper
    {
        public static NetInfo SetRoadLanes(this NetInfo rdInfo, NetInfoVersion version, LanesConfiguration config)
        {
            var template = Prefabs.Find<NetInfo>("Basic Road");
            if (config.VehicleLanesToAdd < 0)
            {
                var remainingLanes = new List<NetInfo.Lane>();
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian || l.m_laneType == NetInfo.LaneType.None || l.m_laneType == NetInfo.LaneType.Parking));
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian && l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking)
                    .Skip(-config.VehicleLanesToAdd));

                rdInfo.m_lanes = remainingLanes.ToArray();
            }
            else if (config.VehicleLanesToAdd > 0)
            {
                var sourceLane = rdInfo.m_lanes.FirstOrDefault(l => l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking && l.m_laneType != NetInfo.LaneType.Pedestrian);
                if (sourceLane == null)
                {
                    sourceLane = template.m_lanes.FirstOrDefault(l => l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking && l.m_laneType != NetInfo.LaneType.Pedestrian);
                }

                var tempLanes = rdInfo.m_lanes.ToList();

                for (var i = 0; i < config.VehicleLanesToAdd; i++)
                {
                    var newLane = sourceLane.CloneWithoutStops();
                    tempLanes.Add(newLane);
                }

                rdInfo.m_lanes = tempLanes.ToArray();
            }

            if (config.PedestrianLanesToAdd < 0)
            {
                var remainingLanes = new List<NetInfo.Lane>();
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType != NetInfo.LaneType.Vehicle && l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking));
                remainingLanes.AddRange(rdInfo
                    .m_lanes
                    .Where(l => l.m_laneType != NetInfo.LaneType.Pedestrian)
                    .Skip(-config.PedestrianLanesToAdd));

                rdInfo.m_lanes = remainingLanes.ToArray();
            }
            else if (config.PedestrianLanesToAdd > 0)
            {
                var sourceLane = rdInfo.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
                if (sourceLane == null)
                {
                    sourceLane = template.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Pedestrian);
                }

                var tempLanes = rdInfo.m_lanes.ToList();

                for (var i = 0; i < config.PedestrianLanesToAdd; i++)
                {
                    var newLane = sourceLane.CloneWithoutStops();
                    tempLanes.Add(newLane);
                }

                rdInfo.m_lanes = tempLanes.ToArray();
            }

            var laneCollection = new List<NetInfo.Lane>();

            laneCollection.AddRange(rdInfo.SetupVehicleLanes(version, config));
            laneCollection.AddRange(rdInfo.SetupPedestrianLanes(version, config));

            if (rdInfo.m_hasParkingSpaces)
            {
                laneCollection.AddRange(rdInfo.SetupParkingLanes());
            }

            var medianLane = rdInfo.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None && l.m_position == 0);
            if (config.CenterLane == CenterLaneType.Median && medianLane != null)
            {
                medianLane = medianLane.SetupMedianLane(config, version);
                laneCollection.Add(medianLane);
            }

            rdInfo.m_lanes = laneCollection.OrderBy(l => l.m_position).ToArray();

            return rdInfo;
        }

        public static void HandleAsymSegmentFlags(NetInfo.Segment fSegment, NetInfo.Segment bSegment = null)
        {
            fSegment.m_forwardForbidden |= NetSegment.Flags.Invert;
            fSegment.m_backwardRequired |= NetSegment.Flags.Invert;
            if (bSegment != null)
            {
                bSegment.m_forwardRequired |= NetSegment.Flags.Invert;
                bSegment.m_backwardForbidden |= NetSegment.Flags.Invert;
            }
        }

        private static IEnumerable<NetInfo.Lane> SetupVehicleLanes(this NetInfo rdInfo, NetInfoVersion version, LanesConfiguration config)
        {
            var isNotSymmetrical = config.LayoutStyle != LanesLayoutStyle.Symmetrical;
            var vehicleLanes = rdInfo.m_lanes
                .Where(l => l.m_laneType != NetInfo.LaneType.None && l.m_laneType != NetInfo.LaneType.Parking && l.m_laneType != NetInfo.LaneType.Pedestrian)
                .ToArray();

            var nbLanes = vehicleLanes.Count();
            var nbUsableLanes = nbLanes - (config.CenterLane == CenterLaneType.TurningLane ? 2 : 0);
            var leftLaneCount = isNotSymmetrical ? (int)Math.Floor((decimal)(int)config.LayoutStyle / 10) : 0;
            var nbLanesBeforeMedian = isNotSymmetrical ? leftLaneCount : nbUsableLanes / 2;
            var positionStart = -(rdInfo.m_halfWidth - rdInfo.m_pavementWidth - (rdInfo.m_hasParkingSpaces ? rdInfo.m_lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.Parking)?.m_width ?? 0 : 0) - (0.5f * config.VehicleLaneWidth) + config.LanePositionOffst);

            for (var i = 0; i < nbLanes; i++)
            {
                var l = vehicleLanes[i].ShallowClone();

                var isTurningLane =
                   config.CenterLane == CenterLaneType.TurningLane &&
                   i >= nbLanesBeforeMedian && i <= nbLanes - nbLanesBeforeMedian - 1;
                var is2ndTurningLane =
                   config.CenterLane == CenterLaneType.TurningLane &&
                   i >= nbLanesBeforeMedian + 1 && i <= nbLanes - nbLanesBeforeMedian - 1;

                if (isTurningLane)
                {
                    l.m_position = 0;
                }
                else
                {
                    if (i < nbLanesBeforeMedian)
                    {
                        l.m_position =
                            positionStart +
                            (i * config.VehicleLaneWidth);
                    }
                    else
                    {
                        if (config.CenterLane == CenterLaneType.Median)
                        {
                            l.m_position =
                                positionStart +
                                (i * config.VehicleLaneWidth) +
                                config.CenterLaneWidth;
                        }
                        else if (config.CenterLane == CenterLaneType.TurningLane)
                        {
                            l.m_position =
                                positionStart +
                                ((i - 2) * config.VehicleLaneWidth) +
                                config.CenterLaneWidth;
                        }
                        else
                        {
                            l.m_position =
                                positionStart +
                                (i * config.VehicleLaneWidth);
                        }
                    }
                }

                //Debug.Log(">>>> Lane Id : " + i + " Position : " + l.m_position);
                l.m_stopType = VehicleInfo.VehicleType.None;
                l.m_width = config.VehicleLaneWidth;

                l.m_laneProps = l.m_laneProps.Clone();
                if (config.SpeedLimit != null && !isTurningLane)
                {
                    l.m_speedLimit = config.SpeedLimit.Value;
                }
                else if (isTurningLane)
                {
                    l.m_speedLimit = 0.6f;
                    l.m_allowConnect = false;
                    SetupTurningLaneProps(l);
                }

                if (config.IsTwoWay)
                {
                    if (isTurningLane)
                    {
                        if (!is2ndTurningLane)
                        {
                            l.m_direction = NetInfo.Direction.Backward;
                        }
                        else
                        {
                            l.m_direction = NetInfo.Direction.Forward;
                        }
                    }
                    else if (isNotSymmetrical)
                    {
                        if (l.m_position <= positionStart + ((leftLaneCount - 1) * l.m_width))
                        {
                            l.m_direction = NetInfo.Direction.Backward;
                        }
                        else
                        {
                            l.m_direction = NetInfo.Direction.Forward;
                        }
                    }
                    else
                    {
                        if (l.m_position < 0.0f)
                        {
                            l.m_direction = NetInfo.Direction.Backward;
                        }
                        else
                        {
                            l.m_direction = NetInfo.Direction.Forward;
                        }

                    }
                }

                foreach (var prop in l.m_laneProps.m_props)
                {
                    prop.m_position = new Vector3(0, 0, -4);
                }
            }

            vehicleLanes = vehicleLanes.OrderBy(l => l.m_position).ToArray();

            // Bus stops configs
            for (int i = 0; i < vehicleLanes.Length; i++)
            {
                var l = vehicleLanes[i].ShallowClone();

                if ((version == NetInfoVersion.Ground || version == NetInfoVersion.GroundGrass || version == NetInfoVersion.GroundTrees) && config.HasBusStop)
                {
                    if (i == 0)
                    {
                        if (config.IsTwoWay)
                        {
                            l.m_stopType = VehicleInfo.VehicleType.Car;
                        }
                        else
                        {
                            l.m_stopType = VehicleInfo.VehicleType.None;
                        }
                    }
                    else if (i == vehicleLanes.Length - 1)
                    {
                        l.m_stopType = VehicleInfo.VehicleType.Car;
                    }
                    else
                    {
                        l.m_stopType = VehicleInfo.VehicleType.None;
                    }

                    if (l.m_stopType == VehicleInfo.VehicleType.Car)
                    {
                        if (l.m_position < 0)
                        {
                            l.m_stopOffset = -config.BusStopOffset;
                        }
                        else
                        {
                            l.m_stopOffset = config.BusStopOffset;
                        }
                    }
                }
                else
                {
                    l.m_stopType = VehicleInfo.VehicleType.None;
                }
            }

            return vehicleLanes;
        }

        public static IEnumerable<NetInfo.Lane> SetupPedestrianLanes(this NetInfo rdInfo, NetInfoVersion version, LanesConfiguration config)
        {
            var pedestrianLanes = rdInfo.m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian)
                .OrderBy(l => l.m_position)
                .ToArray();

            foreach (var pedLane in pedestrianLanes)
            {
                var multiplier = pedLane.m_position / Math.Abs(pedLane.m_position);
                pedLane.m_laneProps = pedLane.m_laneProps.Clone();
                if (config.PedestrianLaneWidth == -1)
                {
                    pedLane.m_width = rdInfo.m_pavementWidth - (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 3 : 1);
                    pedLane.m_position = multiplier * (rdInfo.m_halfWidth - (version == NetInfoVersion.Slope || version == NetInfoVersion.Tunnel ? 2 : 0) - (0.5f * pedLane.m_width) + config.PedLaneOffset);
                }
                else
                {
                    pedLane.m_width = config.PedestrianLaneWidth;
                }


                if (config.PedPropOffsetX != null)
                {
                    foreach (var pedLaneProp in pedLane.m_laneProps.m_props)
                    {
                        pedLaneProp.m_position.x += config.PedPropOffsetX.Value * multiplier;
                    }
                }
            }
            return pedestrianLanes;
        }

        private static IEnumerable<NetInfo.Lane> SetupParkingLanes(this NetInfo rdInfo)
        {
            var parkingLanes = rdInfo
                .m_lanes
                .Where(l => l.m_laneType == NetInfo.LaneType.Parking)
                .ToArray();
            for (int i = 0; i < parkingLanes.Count(); i++)
            {
                parkingLanes[i].m_position = (float)(Math.Sign(parkingLanes[i].m_position) * (rdInfo.m_halfWidth - rdInfo.m_pavementWidth - (0.5 * parkingLanes[i].m_width)));
            }
            return parkingLanes;
        }

        private static NetInfo.Lane SetupMedianLane(this NetInfo.Lane lane, LanesConfiguration config, NetInfoVersion version)
        {
            var laneProps = lane.m_laneProps.Clone();
            for (var i = 0; i < laneProps.m_props.Length; i++)
            {
                var prop = laneProps.m_props[i].ShallowClone();
                if (prop.m_position.x != 0)
                {
                    if (prop.m_prop.name.ToLower().Contains("sign")
                        || prop.m_prop.name.ToLower().Contains("speed limit"))
                    {
                        var multiplier = prop.m_position.z / Math.Abs(prop.m_position.z);
                        prop.m_position = new Vector3(0, 1, 8 * multiplier);
                    }
                    else
                    {
                        var multiplier = prop.m_position.x / Math.Abs(prop.m_position.x);
                        var offset = 0.0f;

                        if (version == NetInfoVersion.Slope)
                        {
                            offset = 0.1f;
                        }
                        else
                        {
                            offset = 0.55f;
                        }
                        prop.m_position.x += multiplier * offset * (config.CenterLaneWidth - lane.m_width);
                    }
                }
                lane.m_laneProps = laneProps;
            }

            return lane;
        }
        private static void SetupTurningLaneProps(NetInfo.Lane lane)
        {
            var isLeftDriving = Singleton<SimulationManager>.instance.m_metaData.m_invertTraffic == SimulationMetaData.MetaBool.True;

            if (lane.m_laneProps == null)
            {
                return;
            }

            if (lane.m_laneProps.m_props == null)
            {
                return;
            }

            var fwd = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Forward);
            var left = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Left);
            var right = lane.m_laneProps.m_props.FirstOrDefault(p => p.m_flagsRequired == NetLane.Flags.Right);

            if (fwd == null)
            {
                return;
            }

            if (left == null)
            {
                return;
            }

            if (right == null)
            {
                return;
            }


            // Existing props
            //var r0 = NetLane.Flags.Forward; 
            //var r1 = NetLane.Flags.ForwardRight;
            //var r2 = NetLane.Flags.Left;
            //var r3 = NetLane.Flags.LeftForward;
            //var r4 = NetLane.Flags.LeftForwardRight;
            //var r5 = NetLane.Flags.LeftRight;
            //var r6 = NetLane.Flags.Right;

            //var f0 = NetLane.Flags.LeftRight;
            //var f1 = NetLane.Flags.Left;
            //var f2 = NetLane.Flags.ForwardRight;
            //var f3 = NetLane.Flags.Right;
            //var f4 = NetLane.Flags.None;
            //var f5 = NetLane.Flags.Forward;
            //var f6 = NetLane.Flags.LeftForward;


            var newProps = new FastList<NetLaneProps.Prop>();

            //newProps.Add(fwd); // Do we want "Forward" on a turning lane?
            newProps.Add(left);
            newProps.Add(right);

            var fl = left.ShallowClone();
            fl.m_flagsRequired = NetLane.Flags.LeftForward;
            fl.m_flagsForbidden = NetLane.Flags.Right;
            newProps.Add(fl);

            var fr = right.ShallowClone();
            fr.m_flagsRequired = NetLane.Flags.ForwardRight;
            fr.m_flagsForbidden = NetLane.Flags.Left;
            newProps.Add(fr);

            var flr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            flr.m_flagsRequired = NetLane.Flags.LeftForwardRight;
            flr.m_flagsForbidden = NetLane.Flags.None;
            newProps.Add(flr);

            var lr = isLeftDriving ? right.ShallowClone() : left.ShallowClone();
            lr.m_flagsRequired = NetLane.Flags.LeftRight;
            lr.m_flagsForbidden = NetLane.Flags.Forward;
            newProps.Add(lr);

            lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            lane.m_laneProps.name = "TurningLane";
            lane.m_laneProps.m_props = newProps.ToArray();
        }
    }
}
