using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synthicate
{
    [CreateAssetMenu(fileName = "BoardManagerSO", menuName = "ScriptableObjects/Board Manager")]
    public class BoardManagerSO : ScriptableObject
    {

        List<PlayerPoint> points;
        List<PlayerEdge> edges;


        // connection lists
        List<uint[]> edge_edge_list;
        List<uint[]> point_edge_list;
        List<uint[]> edge_point_list;
        List<uint[]> point_hex_list;
        List<uint[]> point_point_list;
        List<uint[]> point_depot_list;


        public UnityEvent<List<PlayerPoint>> updatePointsRequestEvent = new UnityEvent<List<PlayerPoint>>();
        public UnityEvent updatePointsResponseEvent = new UnityEvent();
        public UnityEvent<List<PlayerEdge>> updateEdgesRequestEvent = new UnityEvent<List<PlayerEdge>>();
        public UnityEvent updateEdgesResponseEvent = new UnityEvent();
        private void OnEnable()
        {
            //if (updatePointsRequestEvent == null) updatePointsRequestEvent;
            updatePointsRequestEvent.AddListener((List<PlayerPoint> points) => {
                foreach (PlayerPoint point in points) this.points[(int)point.id] = point;
                updatePointsResponseEvent.Invoke();
            });

            updateEdgesRequestEvent.AddListener((List<PlayerEdge> edges) => {
                foreach (PlayerEdge edge in edges) this.edges[(int)edge.id] = edge;
                updateEdgesResponseEvent.Invoke();
            });

            points = new List<PlayerPoint>();
            for (uint i = 0; i < Global.NUM_STRONGHOLD_POINTS; i++) points.Add(new PlayerPoint(i,false, 0, false));

            edges = new List<PlayerEdge>();
            for (uint i = 0; i < Global.NUM_FLYWAY_EDGES; i++) edges.Add(new PlayerEdge(i, false, 0));
            
            connectBoard();
        }


        
        public int getNumOutpostsFor(int player)
        {
            int numOutposts = 0;
            foreach (PlayerPoint point in points) if (point.player == player && point.placed && !point.isStronghold) numOutposts++;
            return numOutposts;
        }
        public int getNumStrongholdsFor(int player)
        {
            int numStrongholds = 0;
            foreach (PlayerPoint point in points) if (point.player == player && point.placed && point.isStronghold) numStrongholds++;
            return numStrongholds;
        }
        public int getNumFlywaysFor(int player)
        {
            int numFlyways = 0;
            foreach (PlayerEdge edge in edges) if (edge.player == player && edge.placed) numFlyways++;
            return numFlyways;
        }
        private List<PlayerPoint> getPlayerPoints(int player)
        {
            List<PlayerPoint> playerPoints = new List<PlayerPoint>();

            foreach (PlayerPoint point in points) if (point.player == player && point.placed) playerPoints.Add(point);

            return playerPoints;
        }

        private int[] getResourcesForPoint(uint point, List<HexResource> hexResources)
        {
            //List<uint> list = null;

            uint[] validHexes = point_hex_list[(int) point];

            int[] pointResources = new int[Global.NUM_RESOURCE_TYPES] {0,0,0,0,0};
            foreach (uint hex in validHexes){
                HexResource hr = hexResources[(int) hex];
                if (hr.hexType != HexType.Desert)
                    pointResources[(int)hr.getResource()] += (int) hr.amount;
            }

            return pointResources;
        }

        public int[] getResourcesForPlayer(int player, List<HexResource> hexResources)
        {
            List<PlayerPoint> playerPoints = getPlayerPoints(player);

            int[] playerResources = new int[Global.NUM_RESOURCE_TYPES] { 0, 0, 0, 0, 0 };

            foreach (PlayerPoint point in playerPoints)
            {
                int[] pointResources = getResourcesForPoint(point.id, hexResources);

                for (int resourceId = 0; resourceId < Global.NUM_RESOURCE_TYPES; resourceId++)
                {
                    int resourceGain = point.isStronghold ? pointResources[resourceId] * 2 : pointResources[resourceId];
                    playerResources[resourceId] += resourceGain;
                }
            }

            return playerResources;
        }

        public List<uint> getValidSetupPointsFor()
        {
            List<uint> validPoints = new List<uint>();

            foreach(PlayerPoint point in points)
            {
                // if this point is placed, do not add it to the list
                if (!point.placed)
                {
                    uint[] neighboringPoints = point_point_list[(int) point.id];

                    bool valid = true;

                    // if the neighbors of this point are placed, do not add it to the list
                    foreach (uint neighboringPoint in neighboringPoints) if (points[(int) neighboringPoint].placed) valid = false;

                    if (valid) validPoints.Add(point.id);
                }
            }

            return validPoints;
        }

        public List<uint> getValidPointsFor(int player)
        {
            List<uint> validPoints = new List<uint>();

            foreach (PlayerPoint point in points)
            {
                // if this point is placed, do not add it to the list
                // unless it is a point owned by this player
                if (!point.placed || (point.placed && point.player == player))
                {
                    uint[] neighboringPoints = point_point_list[(int)point.id];

                    bool noNeighboringOutposts = true;

                    // if the neighbors of this point are placed, do not add it to the list
                    foreach (uint neighboringPoint in neighboringPoints) if (points[(int)neighboringPoint].placed) noNeighboringOutposts = false;

                    // if no neighboring roads are placed, do not add it to the list
                    // TODO: need to create flyway struct with placed attributes
                    bool hasNeighboringRoad = false;
                    uint[] neighboringEdges = point_edge_list[(int)point.id];
                    foreach (uint neighboringEdge in neighboringEdges) if (edges[(int)neighboringEdge].placed && edges[(int)neighboringEdge].player == player) hasNeighboringRoad = true;

                    if (noNeighboringOutposts && hasNeighboringRoad) validPoints.Add(point.id);
                }
            }

            return validPoints;
        }

        public List<uint> getValidEdgesFor(int player)
        {
            List<uint> validEdges = new List<uint>();

            foreach (PlayerEdge edge in edges)
            {
                // if this point is placed, do not add it to the list
                if (!edge.placed)
                {
                    uint[] neighboringPoints = edge_point_list[(int)edge.id];
                    bool hasNeighboringPoint = false;
                    // if the neighbors of this point are placed, do not add it to the list
                    foreach (uint neighboringPoint in neighboringPoints) if (points[(int)neighboringPoint].placed && points[(int)neighboringPoint].player == player) hasNeighboringPoint = true;

                    // if no neighboring roads are placed, do not add it to the list
                    // TODO: need to create flyway struct with placed attributes
                    uint[] neighboringEdges = edge_edge_list[(int)edge.id];
                    bool hasNeighboringRoad = false;
                    foreach (uint neighboringEdge in neighboringEdges) if (edges[(int)neighboringEdge].placed && edges[(int)neighboringEdge].player == player) hasNeighboringRoad = true;

                    if (hasNeighboringPoint || hasNeighboringRoad) validEdges.Add(edge.id);
                }
            }
            // TODO: Renumber flyways
            return validEdges;
        }

        public bool validDepotForPlayer(int player, uint depot)
        {
            foreach (PlayerPoint point in getPlayerPoints(player))
            {
                if (point_depot_list[(int) point.id].Length > 0 && point_depot_list[(int) point.id][0] == depot) return true;
            }
            return false;
        }


        private void connectBoard()
        {
            edge_edge_list = new List<uint[]>();
            edge_point_list = new List<uint[]>();
            point_hex_list = new List<uint[]>();
            point_point_list = new List<uint[]>();
            point_depot_list = new List<uint[]>();

            // edge -> edge connections
            edge_edge_list.Add(new uint[] { 6, 1 }); //road_0
            edge_edge_list.Add(new uint[] { 0, 7, 2 }); //road_1
            edge_edge_list.Add(new uint[] { 1, 7, 3 }); //road_2
            edge_edge_list.Add(new uint[] { 2, 8, 4 }); //road_3
            edge_edge_list.Add(new uint[] { 3, 8, 5 }); //road_4
            edge_edge_list.Add(new uint[] { 4, 9 }); //road_5
            edge_edge_list.Add(new uint[] { 0, 10, 11 }); //road_6
            edge_edge_list.Add(new uint[] { 1, 2, 12, 13 }); //road_7
            edge_edge_list.Add(new uint[] { 3, 4, 14, 15 }); //road_8
            edge_edge_list.Add(new uint[] { 5, 16, 17 }); //road_9
            edge_edge_list.Add(new uint[] { 18, 6, 11 }); //road_10
            edge_edge_list.Add(new uint[] { 6, 10, 12, 19 }); //road_11
            edge_edge_list.Add(new uint[] { 11, 19, 7, 13 }); //road_12
            edge_edge_list.Add(new uint[] { 12, 7, 14, 20 }); //road_13
            edge_edge_list.Add(new uint[] { 13, 20, 8, 15 }); //road_14
            edge_edge_list.Add(new uint[] { 14, 8, 16, 21 }); //road_15
            edge_edge_list.Add(new uint[] { 15, 21, 9, 17 }); //road_16
            edge_edge_list.Add(new uint[] { 16, 9, 22 }); //road_17
            edge_edge_list.Add(new uint[] { 10, 23, 24 }); //road_18
            edge_edge_list.Add(new uint[] { 11, 12, 25, 26 }); //road_19
            edge_edge_list.Add(new uint[] { 13, 14, 27, 28 }); //road_20
            edge_edge_list.Add(new uint[] { 15, 16, 29, 30 }); //road_21
            edge_edge_list.Add(new uint[] { 17, 31, 32 }); //road_22
            edge_edge_list.Add(new uint[] { 18, 33, 24 }); //road_23
            edge_edge_list.Add(new uint[] { 18, 23, 25, 34 }); //road_24
            edge_edge_list.Add(new uint[] { 24, 34, 19, 26 }); //road_25
            edge_edge_list.Add(new uint[] { 25, 19, 27, 35 }); //road_26
            edge_edge_list.Add(new uint[] { 26, 35, 20, 28 }); //road_27
            edge_edge_list.Add(new uint[] { 27, 20, 29, 36 }); //road_28
            edge_edge_list.Add(new uint[] { 28, 36, 21, 30 }); //road_29
            edge_edge_list.Add(new uint[] { 29, 21, 37, 31 }); //road_30
            edge_edge_list.Add(new uint[] { 30, 37, 22, 32 }); //road_31
            edge_edge_list.Add(new uint[] { 22, 31, 38 }); //road_32
            edge_edge_list.Add(new uint[] { 23, 39 }); //road_33
            edge_edge_list.Add(new uint[] { 24, 25, 40, 41 }); //road_34
            edge_edge_list.Add(new uint[] { 26, 27, 42, 43 }); //road_35
            edge_edge_list.Add(new uint[] { 28, 29, 44, 45 }); //road_36
            edge_edge_list.Add(new uint[] { 30, 31, 46, 47 }); //road_37
            edge_edge_list.Add(new uint[] { 32, 48 }); //road_38
            edge_edge_list.Add(new uint[] { 33, 40, 49 }); //road_39
            edge_edge_list.Add(new uint[] { 39, 49, 34, 41 }); //road_40
            edge_edge_list.Add(new uint[] { 34, 40, 42, 50 }); //road_41
            edge_edge_list.Add(new uint[] { 35, 41, 43, 50 }); //road_42
            edge_edge_list.Add(new uint[] { 35, 42, 44, 51 }); //road_43
            edge_edge_list.Add(new uint[] { 36, 43, 45, 51 }); //road_44
            edge_edge_list.Add(new uint[] { 36, 44, 46, 52 }); //road_45
            edge_edge_list.Add(new uint[] { 37, 45, 47, 52 }); //road_46
            edge_edge_list.Add(new uint[] { 37, 46, 48, 53 }); //road_47
            edge_edge_list.Add(new uint[] { 47, 48, 53 }); //road_48
            edge_edge_list.Add(new uint[] { 39, 40, 54 }); //road_49
            edge_edge_list.Add(new uint[] { 41, 42, 55, 56 }); //road_50
            edge_edge_list.Add(new uint[] { 43, 44, 57, 58 }); //road_51
            edge_edge_list.Add(new uint[] { 45, 46, 59, 60 }); //road_52
            edge_edge_list.Add(new uint[] { 47, 48, 61 }); //road_53
            edge_edge_list.Add(new uint[] { 49, 55, 62 }); //road_54
            edge_edge_list.Add(new uint[] { 50, 54, 56, 62 }); //road_55
            edge_edge_list.Add(new uint[] { 50, 55, 57, 63 }); //road_56
            edge_edge_list.Add(new uint[] { 51, 56, 58, 63 }); //road_57
            edge_edge_list.Add(new uint[] { 51, 57, 59, 64 }); //road_58
            edge_edge_list.Add(new uint[] { 52, 58, 60, 64 }); //road_59
            edge_edge_list.Add(new uint[] { 52, 59, 61, 65 }); //road_60
            edge_edge_list.Add(new uint[] { 53, 60, 65 }); //road_61
            edge_edge_list.Add(new uint[] { 54, 55, 66 }); //road_62
            edge_edge_list.Add(new uint[] { 56, 57, 67, 68 }); //road_63
            edge_edge_list.Add(new uint[] { 58, 59, 69, 70 }); //road_64
            edge_edge_list.Add(new uint[] { 60, 61, 65 }); //road_65
            edge_edge_list.Add(new uint[] { 62, 67 }); //road_66
            edge_edge_list.Add(new uint[] { 63, 66, 68 }); //road_67
            edge_edge_list.Add(new uint[] { 63, 67, 68 }); //road_68
            edge_edge_list.Add(new uint[] { 64, 68, 70 }); //road_69
            edge_edge_list.Add(new uint[] { 64, 69, 71 }); //road_70
            edge_edge_list.Add(new uint[] { 65, 71 }); //road_71

            // edge -> point connections
            edge_point_list.Add(new uint[] { 0, 3 }); // road_0
            edge_point_list.Add(new uint[] { 0, 4 }); // road_1
            edge_point_list.Add(new uint[] { 1, 4 }); // road_2
            edge_point_list.Add(new uint[] { 1, 5 }); // road_3
            edge_point_list.Add(new uint[] { 2, 5 }); // road_4
            edge_point_list.Add(new uint[] { 2, 6 }); // road_5
            edge_point_list.Add(new uint[] { 3, 7 }); // road_6
            edge_point_list.Add(new uint[] { 4, 8 }); // road_7
            edge_point_list.Add(new uint[] { 5, 9 }); // road_8
            edge_point_list.Add(new uint[] { 6, 10 }); // road_9
            edge_point_list.Add(new uint[] { 7, 11 }); // road_10
            edge_point_list.Add(new uint[] { 7, 12 }); // road_11
            edge_point_list.Add(new uint[] { 8, 12 }); // road_12
            edge_point_list.Add(new uint[] { 8, 13 }); // road_13
            edge_point_list.Add(new uint[] { 9, 13 }); // road_14
            edge_point_list.Add(new uint[] { 9, 14 }); // road_15
            edge_point_list.Add(new uint[] { 10, 14 }); // road_16
            edge_point_list.Add(new uint[] { 10, 15 }); // road_17
            edge_point_list.Add(new uint[] { 11, 16 }); // road_18
            edge_point_list.Add(new uint[] { 12, 17 }); // road_19
            edge_point_list.Add(new uint[] { 13, 18 }); // road_20
            edge_point_list.Add(new uint[] { 14, 19 }); // road_21
            edge_point_list.Add(new uint[] { 15, 20 }); // road_22
            edge_point_list.Add(new uint[] { 16, 21 }); // road_23
            edge_point_list.Add(new uint[] { 16, 22 }); // road_24
            edge_point_list.Add(new uint[] { 17, 22 }); // road_25
            edge_point_list.Add(new uint[] { 17, 23 }); // road_26
            edge_point_list.Add(new uint[] { 18, 23 }); // road_27
            edge_point_list.Add(new uint[] { 18, 24 }); // road_28
            edge_point_list.Add(new uint[] { 19, 24 }); // road_29
            edge_point_list.Add(new uint[] { 19, 25 }); // road_30
            edge_point_list.Add(new uint[] { 20, 25 }); // road_31
            edge_point_list.Add(new uint[] { 20, 26 }); // road_32
            edge_point_list.Add(new uint[] { 21, 27 }); // road_33
            edge_point_list.Add(new uint[] { 22, 28 }); // road_34
            edge_point_list.Add(new uint[] { 23, 29 }); // road_35
            edge_point_list.Add(new uint[] { 24, 30 }); // road_36
            edge_point_list.Add(new uint[] { 25, 31 }); // road_37
            edge_point_list.Add(new uint[] { 26, 32 }); // road_38
            edge_point_list.Add(new uint[] { 27, 33 }); // road_39
            edge_point_list.Add(new uint[] { 28, 33 }); // road_40
            edge_point_list.Add(new uint[] { 28, 34 }); // road_41
            edge_point_list.Add(new uint[] { 29, 34 }); // road_42
            edge_point_list.Add(new uint[] { 29, 35 }); // road_43
            edge_point_list.Add(new uint[] { 30, 35 }); // road_44
            edge_point_list.Add(new uint[] { 30, 36 }); // road_45
            edge_point_list.Add(new uint[] { 31, 36 }); // road_46
            edge_point_list.Add(new uint[] { 31, 37 }); // road_47
            edge_point_list.Add(new uint[] { 32, 37 }); // road_48
            edge_point_list.Add(new uint[] { 33, 38 }); // road_49
            edge_point_list.Add(new uint[] { 34, 39 }); // road_50
            edge_point_list.Add(new uint[] { 35, 40 }); // road_51
            edge_point_list.Add(new uint[] { 36, 41 }); // road_52
            edge_point_list.Add(new uint[] { 37, 42 }); // road_53
            edge_point_list.Add(new uint[] { 38, 43 }); // road_54
            edge_point_list.Add(new uint[] { 39, 43 }); // road_55
            edge_point_list.Add(new uint[] { 39, 44 }); // road_56
            edge_point_list.Add(new uint[] { 40, 44 }); // road_57
            edge_point_list.Add(new uint[] { 40, 45 }); // road_58
            edge_point_list.Add(new uint[] { 41, 45 }); // road_59
            edge_point_list.Add(new uint[] { 41, 46 }); // road_60
            edge_point_list.Add(new uint[] { 42, 46 }); // road_61
            edge_point_list.Add(new uint[] { 43, 47 }); // road_62
            edge_point_list.Add(new uint[] { 44, 48 }); // road_63
            edge_point_list.Add(new uint[] { 45, 49 }); // road_64
            edge_point_list.Add(new uint[] { 46, 50 }); // road_65
            edge_point_list.Add(new uint[] { 47, 51 }); // road_66
            edge_point_list.Add(new uint[] { 48, 51 }); // road_67
            edge_point_list.Add(new uint[] { 48, 52 }); // road_68
            edge_point_list.Add(new uint[] { 49, 52 }); // road_69
            edge_point_list.Add(new uint[] { 49, 53 }); // road_70
            edge_point_list.Add(new uint[] { 50, 53 }); // road_71

            // create point-edge list
            point_edge_list = new List<uint[]>();
            for (uint point = 0; point < Global.NUM_STRONGHOLD_POINTS; point++)
            {
                List<uint> edges = new List<uint>();
                for (uint edge = 0; edge < Global.NUM_FLYWAY_EDGES; edge++)
                {
                    foreach (uint edge_point in edge_point_list[(int)edge])
                    {
                        if (edge_point == point) edges.Add(edge);
                    }
                }
                point_edge_list.Add(edges.ToArray());
            }

            // create point-point list
            point_point_list = new List<uint[]>();
            for (uint point = 0; point < Global.NUM_STRONGHOLD_POINTS; point++)
            {
                List<uint> neighbor_points = new List<uint>();

                uint[] edges = point_edge_list[(int)point];
                foreach (uint edge in edges)
                {
                    foreach (uint neighbor_point in edge_point_list[(int)edge])
                    {
                        if (neighbor_point != point) neighbor_points.Add(neighbor_point);
                    }
                }

                point_point_list.Add(neighbor_points.ToArray());
            }

            // populate neighboring edges and points for each edge
            //for (int edge_id = 0; edge_id < edge_edge_list.Count; edge_id++)
            //{
            //    foreach (int adj_edge_id in edge_edge_list[edge_id]) this.edgeArray[edge_id].adjacentRoads.Add(this.edgeArray[adj_edge_id]);
            //}

            //for (int edge_id = 0; edge_id < edge_point_list.Count; edge_id++)
            //{
            //    foreach (int adj_point_id in edge_point_list[edge_id])
            //    {
            //        //Debug.Log("Connecting ROAD_" + edge_id + " to point_" + adj_point_id);
            //        this.edgeArray[edge_id].adjacentPoints.Add(this.pointArray[adj_point_id]);

            //        // add edges to points
            //        this.pointArray[adj_point_id].adjacentRoads.Add(edgeArray[edge_id]);
            //    }
            //}

            // point -> hex connections
            point_hex_list.Add(new uint[] { 0 }); // point_0
            point_hex_list.Add(new uint[] { 1 }); // point_1
            point_hex_list.Add(new uint[] { 2 }); // point_2
            point_hex_list.Add(new uint[] { 0 }); // point_3
            point_hex_list.Add(new uint[] { 0, 1 }); // point_4
            point_hex_list.Add(new uint[] { 1, 2 }); // point_5
            point_hex_list.Add(new uint[] { 2 }); // point_6
            point_hex_list.Add(new uint[] { 0, 3 }); // point_7
            point_hex_list.Add(new uint[] { 0, 1, 4 }); // point_8
            point_hex_list.Add(new uint[] { 1, 2, 5 }); // point_9
            point_hex_list.Add(new uint[] { 2, 6 }); // point_10
            point_hex_list.Add(new uint[] { 3 }); // point_11
            point_hex_list.Add(new uint[] { 0, 3, 4 }); // point_12
            point_hex_list.Add(new uint[] { 1, 4, 5 }); // point_13
            point_hex_list.Add(new uint[] { 2, 5, 6 }); // point_14
            point_hex_list.Add(new uint[] { 6 }); // point_15
            point_hex_list.Add(new uint[] { 3, 7 }); // point_16
            point_hex_list.Add(new uint[] { 3, 4, 8 }); // point_17
            point_hex_list.Add(new uint[] { 4, 5, 9 }); // point_18
            point_hex_list.Add(new uint[] { 5, 6, 10 }); // point_19
            point_hex_list.Add(new uint[] { 6, 11 }); // point_20
            point_hex_list.Add(new uint[] { 7 }); // point_21
            point_hex_list.Add(new uint[] { 3, 7, 8 }); // point_22
            point_hex_list.Add(new uint[] { 4, 8, 9 }); // point_23
            point_hex_list.Add(new uint[] { 5, 9, 10 }); // point_24
            point_hex_list.Add(new uint[] { 6, 10, 11 }); // point_25
            point_hex_list.Add(new uint[] { 11 }); // point_26
            point_hex_list.Add(new uint[] { 7 }); // point_27
            point_hex_list.Add(new uint[] { 7, 8, 12 }); // point_28
            point_hex_list.Add(new uint[] { 8, 9, 13 }); // point_29
            point_hex_list.Add(new uint[] { 9, 10, 14 }); // point_30
            point_hex_list.Add(new uint[] { 10, 11, 15 }); // point_31
            point_hex_list.Add(new uint[] { 11 }); // point_32
            point_hex_list.Add(new uint[] { 7, 12 }); // point_33
            point_hex_list.Add(new uint[] { 8, 12, 13 }); // point_34
            point_hex_list.Add(new uint[] { 9, 13, 14 }); // point_35
            point_hex_list.Add(new uint[] { 10, 14, 15 }); // point_36
            point_hex_list.Add(new uint[] { 11, 15 }); // point_37
            point_hex_list.Add(new uint[] { 12 }); // point_38
            point_hex_list.Add(new uint[] { 12, 13, 16 }); // point_39
            point_hex_list.Add(new uint[] { 13, 14, 17 }); // point_40
            point_hex_list.Add(new uint[] { 14, 15, 18 }); // point_41
            point_hex_list.Add(new uint[] { 15 }); // point_42
            point_hex_list.Add(new uint[] { 12, 16 }); // point_43
            point_hex_list.Add(new uint[] { 13, 16, 17 }); // point_44
            point_hex_list.Add(new uint[] { 14, 17, 18 }); // point_45
            point_hex_list.Add(new uint[] { 15, 18 }); // point_46
            point_hex_list.Add(new uint[] { 16 }); // point_47
            point_hex_list.Add(new uint[] { 16, 17 }); // point_48
            point_hex_list.Add(new uint[] { 17, 18 }); // point_49
            point_hex_list.Add(new uint[] { 18 }); // point_50
            point_hex_list.Add(new uint[] { 16 }); // point_51
            point_hex_list.Add(new uint[] { 17 }); // point_52
            point_hex_list.Add(new uint[] { 18 }); // point_53

            //for (int point_id = 0; point_id < point_hex_list.Count; point_id++)
            //{
            //    foreach (int adj_hex_id in point_hex_list[point_id])
            //    {
            //        //Debug.Log("Connecting point_" + point_id + " to HEX_" + adj_hex_id);
            //        this.pointArray[point_id].adjacentHexes.Add(this.hexArray[adj_hex_id]);
            //    }
            //}

            // TODO: Need to setup depot points
            // set ports for given points
            for (int i = 0; i < Global.NUM_STRONGHOLD_POINTS; i++) point_depot_list.Add(new uint[] { });
            point_depot_list[0] = new uint[]{0};
            point_depot_list[3] = new uint[]{0};
            point_depot_list[1] = new uint[]{1};
            point_depot_list[5] = new uint[]{1};
            point_depot_list[10] = new uint[]{2};
            point_depot_list[15] = new uint[]{2};
            point_depot_list[26] = new uint[]{3};
            point_depot_list[32] = new uint[]{3};
            point_depot_list[42] = new uint[]{4};
            point_depot_list[46] = new uint[]{4};
            point_depot_list[49] = new uint[]{5};
            point_depot_list[52] = new uint[]{5};
            point_depot_list[51] = new uint[]{6};
            point_depot_list[47] = new uint[]{6};
            point_depot_list[38] = new uint[]{7};
            point_depot_list[33] = new uint[]{7};
            point_depot_list[16] = new uint[]{8};
            point_depot_list[11] = new uint[]{8};
        }
    }
}
