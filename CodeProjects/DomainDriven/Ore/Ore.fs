namespace EveOnline.OreDomain

module Ore = 
    open EveOnline.ProductDomain.Types
    open EveOnline.OreDomain.Types
    open EveOnline.OreDomain.Records
    
    let RawOreVolume (x:OreType) (y:Compressed) :Volume = 
        Volume <| 
            match x, y with
            | Arkonor,      IsNotCompressed  -> 16.0f
            | Arkonor,      IsCompressed     -> 3.08f

            | Bistot,       IsNotCompressed  -> 16.0f
            | Bistot,       IsCompressed     -> 6.11f

            | Crokite,      IsNotCompressed  -> 16.0f
            | Crokite,      IsCompressed     -> 7.81f

            | DarkOchre,    IsNotCompressed  -> 8.00f
            | DarkOchre,    IsCompressed     -> 3.27f

            | Gneiss,       IsNotCompressed  -> 5.00f
            | Gneiss,       IsCompressed     -> 1.03f

            | Hedbergite,   IsNotCompressed  -> 3.00f
            | Hedbergite,   IsCompressed     -> 0.14f

            | Hemorphite,   IsNotCompressed  -> 3.00f
            | Hemorphite,   IsCompressed     -> 0.16f

            | Jaspet,       IsNotCompressed  -> 2.00f
            | Jaspet,       IsCompressed     -> 0.15f

            | Kernite,      IsNotCompressed  -> 1.20f
            | Kernite,      IsCompressed     -> 0.19f

            | Mercoxit,     IsNotCompressed  -> 40.0f
            | Mercoxit,     IsCompressed     -> 0.10f

            | Omber,        IsNotCompressed  -> 0.60f
            | Omber,        IsCompressed     -> 0.07f

            | Plagioclase,  IsNotCompressed  -> 0.35f
            | Plagioclase,  IsCompressed     -> 0.15f

            | Pyroxeres,    IsNotCompressed  -> 0.30f
            | Pyroxeres,    IsCompressed     -> 0.16f

            | Scordite,     IsNotCompressed  -> 0.15f
            | Scordite,     IsCompressed     -> 0.19f

            | Spodumain,    IsNotCompressed  -> 16.0f
            | Spodumain,    IsCompressed     -> 16.0f

            | Veldspar,     IsNotCompressed  -> 0.10f
            | Veldspar,     IsCompressed     -> 0.15f

    let MinimumRefineQty (x:OreType) :int = // All ore now has refine qty of 100
        match x with
        | _       -> 100

    let RawOreYield (ore:OreType) :OreYield = 
        match ore with
        | Arkonor      ->  { BaseOreYield with Tritanium = Tritanium 6905
                                               Mexallon  = Mexallon  1278
                                               Megacyte  = Megacyte  230 
                                               Zydrine   = Zydrine   115    }
        | Bistot       ->  { BaseOreYield with Pyerite   = Pyerite   16572  
                                               Megacyte  = Megacyte  118    
                                               Zydrine   = Zydrine   236    }
        | Crokite      ->  { BaseOreYield with Tritanium = Tritanium 20992  
                                               Nocxium   = Nocxium   275    
                                               Zydrine   = Zydrine   367    }
        | DarkOchre    ->  { BaseOreYield with Tritanium = Tritanium 8804   
                                               Nocxium   = Nocxium   173    
                                               Zydrine   = Zydrine   87     }
        | Gneiss       ->  { BaseOreYield with Tritanium = Tritanium 1278   
                                               Mexallon  = Mexallon  1278   
                                               Isogen    = Isogen    242    
                                               Zydrine   = Zydrine   60     }
        | Hedbergite   ->  { BaseOreYield with Tritanium = Tritanium 180 
                                               Pyerite   = Pyerite   72  
                                               Mexallon  = Mexallon  17  
                                               Isogen    = Isogen    59  
                                               Nocxium   = Nocxium   118 
                                               Zydrine   = Zydrine   8      }
        | Hemorphite   ->  { BaseOreYield with Pyerite   = Pyerite   81  
                                               Isogen    = Isogen    196 
                                               Nocxium   = Nocxium   98  
                                               Zydrine   = Zydrine   9      }
        | Jaspet       ->  { BaseOreYield with Tritanium = Tritanium 72  
                                               Pyerite   = Pyerite   121 
                                               Mexallon  = Mexallon  144 
                                               Nocxium   = Nocxium   72  
                                               Zydrine   = Zydrine   3      }
        | Kernite      ->  { BaseOreYield with Tritanium = Tritanium 134 
                                               Mexallon  = Mexallon  267 
                                               Isogen    = Isogen    134    }
        | Mercoxit     ->  { BaseOreYield with Morphite  = Morphite  293    }
        | Omber        ->  { BaseOreYield with Tritanium = Tritanium 85  
                                               Pyerite   = Pyerite   34 
                                               Isogen    = Isogen    85     }
        | Plagioclase  ->  { BaseOreYield with Tritanium = Tritanium 107 
                                               Pyerite   = Pyerite   213 
                                               Mexallon  = Mexallon  107    }
        | Pyroxeres    ->  { BaseOreYield with Tritanium = Tritanium 315 
                                               Pyerite   = Pyerite   25  
                                               Mexallon  = Mexallon  50  
                                               Nocxium   = Nocxium   5      }
        | Scordite     ->  { BaseOreYield with Tritanium = Tritanium 346 
                                               Pyerite   = Pyerite   173    }
        | Spodumain    ->  { BaseOreYield with Tritanium = Tritanium 39221
                                               Pyerite   = Pyerite   4972 
                                               Megacyte  = Megacyte  78     }
        | Veldspar     ->  { BaseOreYield with Tritanium = Tritanium 415    }


    let OreData (x:OreType) (y:OreRarity) (z:Compressed) :OreData = 
        fun (x, y) -> { 
            OreId  = TypeId x
            Name   = Name y
        } <| 
            match (x, y, z) with
            | Arkonor,  Common,     IsNotCompressed   -> 22, "Arkonor"     
            | Arkonor,  Uncommon,   IsNotCompressed   -> 17425, "Crimson Arkonor" 
            | Arkonor,  Rare,       IsNotCompressed   -> 17426, "Prime Arkonor" 
            | Arkonor,  Common,     IsCompressed      -> 28367, "Compressed Arkonor" 
            | Arkonor,  Uncommon,   IsCompressed      -> 28385, "Compressed Crimson Arkonor" 
            | Arkonor,  Rare,       IsCompressed      -> 28387, "Compressed Prime Arkonor" 
                             
            | Bistot,  Common,     IsNotCompressed   -> 1223, "Bistot" 
            | Bistot,  Uncommon,   IsNotCompressed   -> 17428, "Triclinic Bistot" 
            | Bistot,  Rare,       IsNotCompressed   -> 17429, "Monoclinic Bistot" 
            | Bistot,  Common,     IsCompressed      -> 28388, "Compressed Bistot" 
            | Bistot,  Uncommon,   IsCompressed      -> 28389, "Compressed Triclinic Bistot" 
            | Bistot,  Rare,       IsCompressed      -> 28390, "Compressed Monoclinic Bistot" 
                               
            | Crokite,  Common,    IsNotCompressed   -> 1225, "Crokite" 
            | Crokite,  Uncommon,  IsNotCompressed   -> 17432, "Sharp Crokite" 
            | Crokite,  Rare,      IsNotCompressed   -> 17433, "Crystalline Crokite" 
            | Crokite,  Common,    IsCompressed      -> 28391, "Compressed Crokite" 
            | Crokite,  Uncommon,  IsCompressed      -> 28392, "Compressed Sharp Crokite" 
            | Crokite,  Rare,      IsCompressed      -> 28393, "Compressed Crystalline Crokite" 
                                    
            | DarkOchre,  Common,    IsNotCompressed   -> 1232, "Dark Ochre" 
            | DarkOchre,  Uncommon,  IsNotCompressed   -> 17436, "Onyx Ochre" 
            | DarkOchre,  Rare,      IsNotCompressed   -> 17437, "Obsidian Ochre" 
            | DarkOchre,  Common,    IsCompressed      -> 28394, "Compressed Dark Ochre" 
            | DarkOchre,  Uncommon,  IsCompressed      -> 28395, "Compressed Onyx Ochre" 
            | DarkOchre,  Rare ,     IsCompressed      -> 28396, "Compressed Obsidian Ochre" 
                                    
            | Gneiss,  Common,    IsNotCompressed   -> 1229, "Gneiss" 
            | Gneiss,  Uncommon,  IsNotCompressed   -> 17865, "Iridescent Gneiss" 
            | Gneiss,  Rare,      IsNotCompressed   -> 17866, "Prismatic Gneiss" 
            | Gneiss,  Common,    IsCompressed      -> 28397, "Compressed Gneiss" 
            | Gneiss,  Uncommon,  IsCompressed      -> 28398, "Compressed Iridescent Gneiss" 
            | Gneiss,  Rare,      IsCompressed      -> 28399, "Compressed Prismatic Gneiss" 
                                    
            | Hedbergite,  Common,    IsNotCompressed   -> 21, "Hedbergite" 
            | Hedbergite,  Uncommon,  IsNotCompressed   -> 17440, "Vitric Hedbergite" 
            | Hedbergite,  Rare,      IsNotCompressed   -> 17441, "Glazed Hedbergite" 
            | Hedbergite,  Common,    IsCompressed      -> 28400, "Compressed Hedbergite" 
            | Hedbergite,  Uncommon,  IsCompressed      -> 28401, "Compressed Vitric Hedbergite" 
            | Hedbergite,  Rare,      IsCompressed      -> 28402, "Compressed Glazed Hedbergite" 
                                    
            | Hemorphite,  Common,    IsNotCompressed   -> 1231, "Hemorphite" 
            | Hemorphite,  Uncommon,  IsNotCompressed   -> 17444, "Vivid Hemorphite" 
            | Hemorphite,  Rare,      IsNotCompressed   -> 17445, "Radiant Hemorphite" 
            | Hemorphite,  Common,    IsCompressed      -> 28403, "Compressed Hemorphite" 
            | Hemorphite,  Uncommon,  IsCompressed      -> 28404, "Compressed Vivid Hemorphite" 
            | Hemorphite,  Rare,      IsCompressed      -> 28405, "Compressed Radiant Hemorphite" 
                                    
            | Jaspet,  Common,    IsNotCompressed   -> 1226, "Jaspet" 
            | Jaspet,  Uncommon,  IsNotCompressed   -> 17448, "Pure Jaspet" 
            | Jaspet,  Rare,      IsNotCompressed   -> 17449, "Pristine Jaspet" 
            | Jaspet,  Common,    IsCompressed      -> 28406, "Compressed Jaspet" 
            | Jaspet,  Uncommon,  IsCompressed      -> 28407, "Compressed Pure Jaspet" 
            | Jaspet,  Rare,      IsCompressed      -> 28408, "Compressed Pristine Jaspet" 
                                    
            | Kernite,  Common,    IsNotCompressed   -> 20, "Kernite" 
            | Kernite,  Uncommon,  IsNotCompressed   -> 17452, "Luminous Kernite" 
            | Kernite,  Rare,      IsNotCompressed   -> 17453, "Fiery Kernite" 
            | Kernite,  Common,    IsCompressed      -> 28410, "Compressed Kernite" 
            | Kernite,  Uncommon,  IsCompressed      -> 28411, "Compressed Luminous Kernite" 
            | Kernite,  Rare,      IsCompressed      -> 28409, "Compressed Fiery Kernite" 
                                    
            | Mercoxit,  Common,    IsNotCompressed   -> 11396, "Mercoxit" 
            | Mercoxit,  Uncommon,  IsNotCompressed   -> 17869, "Magma Mercoxit" 
            | Mercoxit,  Rare,      IsNotCompressed   -> 17870, "Vitreous Mercoxit" 
            | Mercoxit,  Common,    IsCompressed      -> 28413, "Compressed Mercoxit" 
            | Mercoxit,  Uncommon,  IsCompressed      -> 28412, "Compressed Magma Mercoxit" 
            | Mercoxit,  Rare,      IsCompressed      -> 28414, "Compressed Vitreous Mercoxit" 
                                    
            | Omber,  Common,    IsNotCompressed   -> 1227, "Omber" 
            | Omber,  Uncommon,  IsNotCompressed   -> 17867, "Silvery Omber" 
            | Omber,  Rare,      IsNotCompressed   -> 17868, "Golden Omber" 
            | Omber,  Common,    IsCompressed      -> 28416, "Compressed Omber" 
            | Omber,  Uncommon,  IsCompressed      -> 28417, "Compressed Silvery Omber" 
            | Omber,  Rare,      IsCompressed      -> 28415, "Compressed Golden Omber" 
                                    
            | Plagioclase,  Common,    IsNotCompressed   -> 18, "Plagioclase" 
            | Plagioclase,  Uncommon,  IsNotCompressed   -> 17455, "Azure Plagioclase" 
            | Plagioclase,  Rare,      IsNotCompressed   -> 17456, "Rich Plagioclase" 
            | Plagioclase,  Common,    IsCompressed      -> 28422, "Compressed Plagioclase" 
            | Plagioclase,  Uncommon,  IsCompressed      -> 28421, "Compressed Azure Plagioclase" 
            | Plagioclase,  Rare,      IsCompressed      -> 28423, "Compressed Rich Plagioclase" 
                                    
            | Pyroxeres,  Common,    IsNotCompressed   -> 1224, "Pyroxeres" 
            | Pyroxeres,  Uncommon,  IsNotCompressed   -> 17459, "Solid Pyroxeres" 
            | Pyroxeres,  Rare,      IsNotCompressed   -> 17460, "Viscous Pyroxeres" 
            | Pyroxeres,  Common,    IsCompressed      -> 28424, "Compressed Pyroxeres"
            | Pyroxeres,  Uncommon,  IsCompressed      -> 28425, "Compressed Solid Pyroxeres"
            | Pyroxeres,  Rare,      IsCompressed      -> 28426, "Compressed Viscous Pyroxeres"
                                    
            | Scordite,  Common,     IsNotCompressed   -> 1228, "Scordite" 
            | Scordite,  Uncommon,   IsNotCompressed   -> 17463, "Condensed Scordite"
            | Scordite,  Rare,       IsNotCompressed   -> 17464, "Massive Scordite"  
            | Scordite,  Common,     IsCompressed      -> 28427, "Compressed Scordite" 
            | Scordite,  Uncommon,   IsCompressed      -> 28428, "Compressed Condensed Scordite" 
            | Scordite,  Rare,       IsCompressed      -> 28429, "Compressed Massive Scordite" 
                                    
            | Spodumain,  Common,    IsNotCompressed   -> 19, "Spodumain" 
            | Spodumain,  Uncommon,  IsNotCompressed   -> 17466, "Bright Spodumain" 
            | Spodumain,  Rare,      IsNotCompressed   -> 17467, "Gleaming Spodumain" 
            | Spodumain,  Common,    IsCompressed      -> 28420, "Compressed Spodumain" 
            | Spodumain,  Uncommon,  IsCompressed      -> 28418, "Compressed Bright Spodumain" 
            | Spodumain,  Rare,      IsCompressed      -> 28419, "Compressed Gleaming Spodumain" 
                                    
            | Veldspar,  Common,    IsNotCompressed   -> 1230, "Veldspar" 
            | Veldspar,  Uncommon,  IsNotCompressed   -> 17470, "Concentrated Veldspar" 
            | Veldspar,  Rare,      IsNotCompressed   -> 17471, "Dense Veldspar" 
            | Veldspar,  Common,    IsCompressed      -> 28430, "Compressed Veldspar" 
            | Veldspar,  Uncommon,  IsCompressed      -> 28431, "Compressed Concentrated Veldspar"
            | Veldspar,  Rare,      IsCompressed      -> 28432, "Compressed Dense Veldspar" 
            

    let OreCategoryName (x:OreType) :Name = 
        Name <| 
            match x with
            | Veldspar      -> "Veldspar"
            | Scordite      -> "Scordite"
            | Pyroxeres     -> "Pyroxeres"
            | Plagioclase   -> "Plagioclase"
            | Omber         -> "Omber"
            | Kernite       -> "Kernite"
            | Jaspet        -> "Jaspet"
            | Hedbergite    -> "Hedbergite"
            | Hemorphite    -> "Hemorphite"
            | Gneiss        -> "Gneiss"
            | DarkOchre     -> "Dark Ochre"
            | Spodumain     -> "Spodumain"
            | Arkonor       -> "Arkonor"
            | Crokite       -> "Crokite"
            | Bistot        -> "Bistot"
            | Mercoxit      -> "Mercoxit"     


    let OreFactory (r:OreRarity) (c:Compressed) (q:Qty) (n:OreType) :RawOre= 
        OreData (n) (r) (c) |> fun x -> 
        {
            Name    = x.Name
            OreId   = x.OreId
            OreType = n 
            Qty     = q
            Yield   = RawOreYield n 
            Volume  = RawOreVolume n c
        }


