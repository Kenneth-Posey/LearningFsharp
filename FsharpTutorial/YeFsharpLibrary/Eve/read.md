﻿Notes: 
===

There's four values per ore type to work with:
- Comp highest buy / lowest sell
- Raw  highest buy / lowest sell

The profit is sell 1 compressed - buy * 100 raw

There's two potential sell values
- Comp highest buy / lowest sell
There's two potential buy values
- Raw  highest buy / lowest sell

Slow (max profit)
Cost   : buy  raw  at highest buy + 0.01
Profit : sell comp at lowest sell - 0.01

Fast (instant profit)
Cost   : buy  raw  at lowest sell (real cost)
Profit : sell comp at highest buy (real profit)

We want to know the profit per unit and per m^3
Compressed ores require 1 unit to refine, uncompressed require 100

Refining formula: 
Equipment base                           max 0.60
 x (1 + processing skill x 0.03)         max 1.15
 x (1 + processing efficiency x 0.02)    max 1.10
 x (1 + ore processing x 0.02)           max 1.10
 x (1 + processing implant)              max 1.04

Best yield  0.8683  lvl V with implant    at 60% station
Best yield  0.8349  lvl V with no implant at 60% station (15% more than pos)
POS yield   0.7525  lvl V with implant    at 52% station (4%  more than no implant)
POS yield   0.7235  lvl V with no implant at 52% station (10% more than npc)
NPC yield   0.6609  lvl V with no implant at 50% station (with tax)