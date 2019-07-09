# Speja

![Alt text](Capture.png?raw=true "Layers")

## What is it?
A fast force directed neo4j visualizer with both 2D and 3D projections. Features a proprietary multithreaded (C# Jobs, Burst Compiled) Fruchterman-Reingold force algorithm implementation.

## Getting started
Fire it up and set the server credentials on the Neo4jServer GameObject.

Hit AddNeo4jNodes on the GraphData object.

Check UseLayerSeparation the GraphInstantitor to separate nodes by type in the Y axis. Click Respawn to activate. Set layers by type on the NodeTypes GameObject.

Space or Middle Mouse switches between projections (2D/3D).

## Controls

Mouse and WASD. Left click to drag nodes around and make them stationary. Click again to let them go.

Right click to orbit a node. Click another node or into space to let go.

![Alt text](layers.png?raw=true "Layers")