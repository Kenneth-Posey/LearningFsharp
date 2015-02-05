namespace EveOnline.Geography

module Celestial =     
    
    type CelestialType = 
    | Star
    | Planet
    | Moon
    | Stargate
    
    type Deployable = 
    | MobileTractorUnit
    | MobileDepot
    | LargeWarpBubble
    | MediumWarpBubble
    | SmallWarpBubble
    | ProbeScreen
    | MicroJump
    | Container
