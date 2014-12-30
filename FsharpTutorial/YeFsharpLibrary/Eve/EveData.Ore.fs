namespace EveData

module Ore = 
    open EveData.RawMaterial
    open EveData.RawMaterialTypes
    open EveData.RawMaterialRecords

    // Old OOP style version
    module Types =   
        type SimpleOre = {
            Name   : string
            TypeId : int
            Value  : single
            IsTiny : bool
        }
        
        type OreYield = {
            Tritanium   : int
            Pyerite     : int
            Mexallon    : int
            Isogen      : int
            Nocxium     : int
            Megacyte    : int
            Zydrine     : int
            Morphite    : int
        }

        let BaseOreYield = {
            Tritanium   = 0
            Pyerite     = 0
            Mexallon    = 0
            Isogen      = 0
            Nocxium     = 0
            Megacyte    = 0
            Zydrine     = 0
            Morphite    = 0
        }
        

        // The actual prices for the minerals
        type OreValue = {
            Tritanium   : single
            Pyerite     : single
            Mexallon    : single
            Isogen      : single
            Nocxium     : single
            Megacyte    : single
            Zydrine     : single
            Morphite    : single
        }


        type IOre = 
            inherit IRawMat<OreYield>
            abstract member GetBase5  : unit -> int
            abstract member GetBase10 : unit -> int
            abstract member GetName5  : unit -> string
            abstract member GetName10 : unit -> string


        type IRawOre =
            inherit IOre


        type ICompressedOre =
            inherit IRawOre


    module Functions = 
        open Types
        let FactorOreYield (baseYield:OreYield) (factor:single) =
            let multiply x = int (single x * factor)
            {   
                BaseOreYield with
                Tritanium   = multiply baseYield.Tritanium
                Pyerite     = multiply baseYield.Pyerite  
                Mexallon    = multiply baseYield.Mexallon 
                Isogen      = multiply baseYield.Isogen   
                Nocxium     = multiply baseYield.Nocxium  
                Megacyte    = multiply baseYield.Megacyte 
                Zydrine     = multiply baseYield.Zydrine
                Morphite    = multiply baseYield.Morphite
            }

        let CompressedYield (baseYield:OreYield) =
            FactorOreYield baseYield 100.0f

        
        let RefineValueOre (item:RawOre) (value:OreValue) =
            let mineralYield = item.Yield
            (   single mineralYield.Isogen.Value    * value.Isogen 
              + single mineralYield.Megacyte.Value  * value.Megacyte
              + single mineralYield.Mexallon.Value  * value.Mexallon
              + single mineralYield.Morphite.Value  * value.Morphite
              + single mineralYield.Nocxium.Value   * value.Nocxium
              + single mineralYield.Pyerite.Value   * value.Pyerite
              + single mineralYield.Tritanium.Value * value.Tritanium
              + single mineralYield.Zydrine.Value   * value.Zydrine
            ) 

               
    module RawMaterials =  
        open Functions      
        open Types   

        type Veldspar () =
            interface IRawOre with
                override this.GetName () = "Veldspar"
                override this.GetName5 () = "Concentrated Veldspar"
                override this.GetName10 () = "Dense Veldspar"
                override this.GetBase () = Veldspar.Base
                override this.GetBase5 () = Veldspar.Base5
                override this.GetBase10 () = Veldspar.Base10
                override this.IsTiny () = false
                override this.GetYield () = Veldspar.Yield
                override this.GetVolume () = Veldspar.Volume

            static member val Base   = 1230  with get
            static member val Base5  = 17470 with get
            static member val Base10 = 17471 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 415
                } with get
            static member val Volume = 0.1f with get

        
        type Scordite () =
            interface IRawOre with
                override this.GetName () = "Scordite"
                override this.GetName5 () = "Condensed Scordite"
                override this.GetName10 () = "Massive Scordite"
                override this.GetBase () = Scordite.Base
                override this.GetBase5 () = Scordite.Base5
                override this.GetBase10 () = Scordite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Scordite.Yield
                override this.GetVolume () = Scordite.Volume

            static member val Base   = 1228  with get
            static member val Base5  = 17463 with get
            static member val Base10 = 17464 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 346
                    Pyerite     = 173
                } with get
            static member val Volume = 0.15f with get
        

        type Pyroxeres () =
            interface IRawOre with
                override this.GetName () = "Pyroxeres"
                override this.GetName5 () = "Solid Pyroxeres"
                override this.GetName10 () = "Viscous Pyroxeres"
                override this.GetBase () = Pyroxeres.Base
                override this.GetBase5 () = Pyroxeres.Base5
                override this.GetBase10 () = Pyroxeres.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Pyroxeres.Yield
                override this.GetVolume () = Pyroxeres.Volume

            static member val Base   = 1224  with get
            static member val Base5  = 17459 with get
            static member val Base10 = 17460 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 315
                    Pyerite     = 25
                    Mexallon    = 50
                    Nocxium     = 5
                } with get
            static member val Volume = 0.3f with get
        
        
        type Plagioclase () =
            interface IRawOre with
                override this.GetName () = "Plagioclase"
                override this.GetName5 () = "Azure Plagioclase"
                override this.GetName10 () = "Rich Plagioclase"
                override this.GetBase () = Plagioclase.Base
                override this.GetBase5 () = Plagioclase.Base5
                override this.GetBase10 () = Plagioclase.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Plagioclase.Yield
                override this.GetVolume () = Plagioclase.Volume

            static member val Base   = 18 with get
            static member val Base5  = 17455 with get
            static member val Base10 = 17456 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 107
                    Pyerite     = 213
                    Mexallon    = 107
                } with get
            static member val Volume = 0.35f with get


        type Omber () =
            interface IRawOre with
                override this.GetName () = "Omber"
                override this.GetName5 () = "Silvery Omber"
                override this.GetName10 () = "Golden Omber"
                override this.GetBase () = Omber.Base
                override this.GetBase5 () = Omber.Base5
                override this.GetBase10 () = Omber.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Omber.Yield
                override this.GetVolume () = Omber.Volume

            static member val Base   = 1227 with get
            static member val Base5  = 17867 with get
            static member val Base10 = 17868 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 85
                    Pyerite     = 34
                    Isogen      = 85
                } with get
            static member val Volume = 0.6f with get


        type Kernite () =
            interface IRawOre with
                override this.GetName () = "Kernite"
                override this.GetName5 () = "Luminous Kernite"
                override this.GetName10 () = "Fiery Kernite"
                override this.GetBase () = Kernite.Base
                override this.GetBase5 () = Kernite.Base5
                override this.GetBase10 () = Kernite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Kernite.Yield
                override this.GetVolume () = Kernite.Volume

            static member val Base   = 20 with get
            static member val Base5  = 17452 with get
            static member val Base10 = 17453 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 134
                    Mexallon    = 267
                    Isogen      = 134
                } with get
            static member val Volume = 1.2f with get
        

        type Jaspet () = 
            interface IRawOre with
                override this.GetName () = "Jaspet"
                override this.GetName5 () = "Pure Jaspet"
                override this.GetName10 () = "Pristine Jaspet"
                override this.GetBase () = Jaspet.Base
                override this.GetBase5 () = Jaspet.Base5
                override this.GetBase10 () = Jaspet.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Jaspet.Yield
                override this.GetVolume () = Jaspet.Volume

            static member val Base   = 1226  with get
            static member val Base5  = 17448 with get
            static member val Base10 = 17449 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 72
                    Pyerite     = 121
                    Mexallon    = 144
                    Nocxium     = 72
                    Zydrine     = 3
                } with get
            static member val Volume = 2.0f with get
       

        type Hemorphite () = 
            interface IRawOre with
                override this.GetName () = "Hemorphite"
                override this.GetName5 () = "Vivid Hemorphite"
                override this.GetName10 () = "Radiant Hemorphite"
                override this.GetBase () = Hemorphite.Base
                override this.GetBase5 () = Hemorphite.Base5
                override this.GetBase10 () = Hemorphite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Hemorphite.Yield
                override this.GetVolume () = Hemorphite.Volume

            static member val Base   = 1231  with get
            static member val Base5  = 17444 with get
            static member val Base10 = 17445 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 180
                    Pyerite     = 72
                    Mexallon    = 17
                    Isogen      = 59
                    Nocxium     = 118
                    Zydrine     = 8
                } with get
            static member val Volume = 3.0f with get
            

        type Hedbergite () =
            interface IRawOre with
                override this.GetName () = "Hedbergite"
                override this.GetName5 () = "Vitric Hedbergite"
                override this.GetName10 () = "Glazed Hedbergite"
                override this.GetBase () = Hedbergite.Base
                override this.GetBase5 () = Hedbergite.Base5
                override this.GetBase10 () = Hedbergite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Hedbergite.Yield
                override this.GetVolume () = Hedbergite.Volume

            static member val Base   = 21    with get
            static member val Base5  = 17440 with get
            static member val Base10 = 17441 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Pyerite     = 81
                    Isogen      = 196
                    Nocxium     = 98
                    Zydrine     = 9
                } with get
            static member val Volume = 3.0f with get


        type Gneiss () =
            interface IRawOre with
                override this.GetName () = "Gneiss"
                override this.GetName5 () = "Iridescent Gneiss"
                override this.GetName10 () = "Prismatic Gneiss"
                override this.GetBase () = Gneiss.Base
                override this.GetBase5 () = Gneiss.Base5
                override this.GetBase10 () = Gneiss.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Gneiss.Yield
                override this.GetVolume () = Gneiss.Volume

            static member val Base   = 1229 with get
            static member val Base5  = 17865 with get
            static member val Base10 = 17866 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 1278
                    Mexallon    = 1278
                    Isogen      = 242
                    Zydrine     = 60
                } with get
            static member val Volume = 5.0f with get


        type DarkOchre () =
            interface IRawOre with
                override this.GetName () = "Dark Ochre"
                override this.GetName5 () = "Onyx Ochre"
                override this.GetName10 () = "Obsidian Ochre"
                override this.GetBase () = DarkOchre.Base
                override this.GetBase5 () = DarkOchre.Base5
                override this.GetBase10 () = DarkOchre.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = DarkOchre.Yield
                override this.GetVolume () = DarkOchre.Volume

            static member val Base   = 1232 with get
            static member val Base5  = 17436 with get
            static member val Base10 = 17437 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 8804
                    Nocxium     = 173
                    Zydrine     = 87
                } with get
            static member val Volume = 8.0f with get


        type Spodumain () =
            interface IRawOre with
                override this.GetName () = "Spodumain"
                override this.GetName5 () = "Bright Spodumain"
                override this.GetName10 () = "Gleaming Spodumain"
                override this.GetBase () = Spodumain.Base
                override this.GetBase5 () = Spodumain.Base5
                override this.GetBase10 () = Spodumain.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Spodumain.Yield
                override this.GetVolume () = Spodumain.Volume

            static member val Base   = 19 with get
            static member val Base5  = 17466 with get
            static member val Base10 = 17467 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 39221
                    Pyerite     = 4972
                    Megacyte    = 78
                } with get
            static member val Volume = 16.0f with get


        type Arkonor () =
            interface IRawOre with
                override this.GetName () = "Arkonor"
                override this.GetName5 () = "Crimson Arkonor"
                override this.GetName10 () = "Prime Arkonor"
                override this.GetBase () = Arkonor.Base
                override this.GetBase5 () = Arkonor.Base5
                override this.GetBase10 () = Arkonor.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Arkonor.Yield
                override this.GetVolume () = Arkonor.Volume

            static member val Base   = 22 with get
            static member val Base5  = 17425 with get
            static member val Base10 = 17426 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 6905
                    Mexallon    = 1278
                    Megacyte    = 230
                    Zydrine     = 115
                } with get
            static member val Volume = 16.0f with get


        type Bistot () =
            interface IRawOre with
                override this.GetName () = "Bistot"
                override this.GetName5 () = "Triclinic Bistot"
                override this.GetName10 () = "Monoclinic Bistot"
                override this.GetBase () = Bistot.Base
                override this.GetBase5 () = Bistot.Base5
                override this.GetBase10 () = Bistot.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Bistot.Yield
                override this.GetVolume () = Bistot.Volume

            static member val Base   = 1223 with get
            static member val Base5  = 17428 with get
            static member val Base10 = 17429 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Pyerite     = 16572
                    Megacyte    = 118
                    Zydrine     = 236
                } with get
            static member val Volume = 16.0f with get


        type Crokite () =
            interface IRawOre with
                override this.GetName () = "Crokite"
                override this.GetName5 () = "Sharp Crokite"
                override this.GetName10 () = "Crystalline Crokite"
                override this.GetBase () = Crokite.Base
                override this.GetBase5 () = Crokite.Base5
                override this.GetBase10 () = Crokite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Crokite.Yield
                override this.GetVolume () = Crokite.Volume

            static member val Base   = 1225 with get
            static member val Base5  = 17432 with get
            static member val Base10 = 17433 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Tritanium   = 20992
                    Nocxium     = 275
                    Zydrine     = 367
                } with get
            static member val Volume = 16.0f with get


        type Mercoxit () =
            interface IRawOre with
                override this.GetName () = "Mercoxit"
                override this.GetName5 () = "Magma Mercoxit"
                override this.GetName10 () = "Vitreous Mercoxit"
                override this.GetBase () = Mercoxit.Base
                override this.GetBase5 () = Mercoxit.Base5
                override this.GetBase10 () = Mercoxit.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = Mercoxit.Yield
                override this.GetVolume () = Mercoxit.Volume

            static member val Base   = 11396 with get
            static member val Base5  = 17869 with get
            static member val Base10 = 17870 with get
            static member val Yield  = { 
                    BaseOreYield with
                    Morphite    = 293
                } with get
            static member val Volume = 40.0f with get


        type CompVeldspar () =
            interface ICompressedOre with
                override this.GetName () = "Compressed Veldspar"
                override this.GetName5 () = "Compressed Concentrated Veldspar"
                override this.GetName10 () = "Compressed Dense Veldspar"
                override this.GetBase () = CompVeldspar.Base
                override this.GetBase5 () = CompVeldspar.Base5
                override this.GetBase10 () = CompVeldspar.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompVeldspar.Yield
                override this.GetVolume () = CompVeldspar.Volume

            static member val Base   = 28430 with get
            static member val Base5  = 28431 with get
            static member val Base10 = 28432 with get
            static member val Yield  = CompressedYield Veldspar.Yield with get
            static member val Volume = 0.15f with get


        type CompScordite () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Scordite"
                override this.GetName5 () =  "Compressed Condensed Scordite"
                override this.GetName10 () = "Compressed Massive Scordite"
                override this.GetBase () = CompScordite.Base
                override this.GetBase5 () = CompScordite.Base5
                override this.GetBase10 () = CompScordite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompScordite.Yield
                override this.GetVolume () = CompScordite.Volume

            static member val Base   = 28427 with get
            static member val Base5  = 28428 with get
            static member val Base10 = 28429 with get
            static member val Yield  = CompressedYield Scordite.Yield with get
            static member val Volume = 0.19f with get
                                                 

        type CompPyroxeres () =       
            interface ICompressedOre with           
                override this.GetName () =   "Compressed Pyroxeres"
                override this.GetName5 () =  "Compressed Solid Pyroxeres"
                override this.GetName10 () = "Compressed Viscous Pyroxeres"
                override this.GetBase () = CompPyroxeres.Base
                override this.GetBase5 () = CompPyroxeres.Base5
                override this.GetBase10 () = CompPyroxeres.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompPyroxeres.Yield
                override this.GetVolume () = CompPyroxeres.Volume

            static member val Base   = 28424 with get
            static member val Base5  = 28425 with get
            static member val Base10 = 28426 with get
            static member val Yield  = CompressedYield Pyroxeres.Yield with get
            static member val Volume = 0.16f with get
            
        
        type CompPlagioclase () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Plagioclase"
                override this.GetName5 () =  "Compressed Azure Plagioclase"
                override this.GetName10 () = "Compressed Rich Plagioclase"
                override this.GetBase () = CompPlagioclase.Base
                override this.GetBase5 () = CompPlagioclase.Base5
                override this.GetBase10 () = CompPlagioclase.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompPlagioclase.Yield
                override this.GetVolume () = CompPlagioclase.Volume

            static member val Base   = 28422 with get
            static member val Base5  = 28421 with get
            static member val Base10 = 28423 with get
            static member val Yield  = CompressedYield Plagioclase.Yield with get
            static member val Volume = 0.15f with get


        type CompOmber () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Omber"
                override this.GetName5 () =  "Compressed Silvery Omber"
                override this.GetName10 () = "Compressed Golden Omber"
                override this.GetBase () = CompOmber.Base
                override this.GetBase5 () = CompOmber.Base5
                override this.GetBase10 () = CompOmber.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompOmber.Yield
                override this.GetVolume () = CompOmber.Volume

            static member val Base   = 28416 with get
            static member val Base5  = 28417 with get
            static member val Base10 = 28415 with get
            static member val Yield  = CompressedYield Omber.Yield with get
            static member val Volume = 0.07f with get


        type CompKernite () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Kernite"
                override this.GetName5 () =  "Compressed Luminous Kernite"
                override this.GetName10 () = "Compressed Fiery Kernite"
                override this.GetBase () = CompKernite.Base
                override this.GetBase5 () = CompKernite.Base5
                override this.GetBase10 () = CompKernite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompKernite.Yield
                override this.GetVolume () = CompKernite.Volume

            static member val Base   = 28410 with get
            static member val Base5  = 28411 with get
            static member val Base10 = 28409 with get
            static member val Yield  = CompressedYield Kernite.Yield with get
            static member val Volume = 0.19f with get
                                                 

        type CompJaspet () =                
            interface ICompressedOre with
                override this.GetName () =   "Compressed Jaspet"
                override this.GetName5 () =  "Compressed Pure Jaspet"
                override this.GetName10 () = "Compressed Pristine Jaspet"
                override this.GetBase () = CompJaspet.Base
                override this.GetBase5 () = CompJaspet.Base5
                override this.GetBase10 () = CompJaspet.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompJaspet.Yield
                override this.GetVolume () = CompJaspet.Volume
        
            static member val Base   = 28406 with get
            static member val Base5  = 28407 with get
            static member val Base10 = 28408 with get
            static member val Yield  = CompressedYield Jaspet.Yield with get
            static member val Volume = 0.15f with get
                                                 

        type CompHemorphite () =             
            interface ICompressedOre with     
                override this.GetName () =   "Compressed Hemorphite"
                override this.GetName5 () =  "Compressed Vivid Hemorphite"
                override this.GetName10 () = "Compressed Radiant Hemorphite"
                override this.GetBase () = CompHemorphite.Base
                override this.GetBase5 () = CompHemorphite.Base5
                override this.GetBase10 () = CompHemorphite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompHemorphite.Yield
                override this.GetVolume () = CompHemorphite.Volume

            static member val Base   = 28403 with get
            static member val Base5  = 28404 with get
            static member val Base10 = 28405 with get
            static member val Yield  = CompressedYield Hemorphite.Yield with get
            static member val Volume = 0.16f with get


        type CompHedbergite () =        
            interface ICompressedOre with          
                override this.GetName () =   "Compressed Hedbergite"
                override this.GetName5 () =  "Compressed Vitric Hedbergite"
                override this.GetName10 () = "Compressed Glazed Hedbergite"
                override this.GetBase () = CompHedbergite.Base
                override this.GetBase5 () = CompHedbergite.Base5
                override this.GetBase10 () = CompHedbergite.Base10
                override this.IsTiny () = true
                override this.GetYield ()  = CompHedbergite.Yield
                override this.GetVolume () = CompHedbergite.Volume

            static member val Base   = 28400 with get
            static member val Base5  = 28401 with get
            static member val Base10 = 28402 with get
            static member val Yield  = CompressedYield Hedbergite.Yield with get
            static member val Volume = 0.14f with get


        type CompGneiss () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Gneiss"
                override this.GetName5 () =  "Compressed Iridescent Gneiss"
                override this.GetName10 () = "Compressed Prismatic Gneiss"
                override this.GetBase () = CompGneiss.Base
                override this.GetBase5 () = CompGneiss.Base5
                override this.GetBase10 () = CompGneiss.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompGneiss.Yield
                override this.GetVolume () = CompGneiss.Volume

            static member val Base   = 28397 with get
            static member val Base5  = 28398 with get
            static member val Base10 = 28399 with get
            static member val Yield  = CompressedYield Gneiss.Yield with get
            static member val Volume = 1.03f with get


        type CompDarkOchre () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Dark Ochre"
                override this.GetName5 () =  "Compressed Onyx Ochre"
                override this.GetName10 () = "Compressed Obsidian Ochre"
                override this.GetBase () = CompDarkOchre.Base
                override this.GetBase5 () = CompDarkOchre.Base5
                override this.GetBase10 () = CompDarkOchre.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompDarkOchre.Yield
                override this.GetVolume () = CompDarkOchre.Volume

            static member val Base   = 28394 with get
            static member val Base5  = 28395 with get
            static member val Base10 = 28396 with get
            static member val Yield  = CompressedYield DarkOchre.Yield with get
            static member val Volume = 3.27f with get


        type CompSpodumain () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Spodumain"
                override this.GetName5 () =  "Compressed Bright Spodumain"
                override this.GetName10 () = "Compressed Gleaming Spodumain"
                override this.GetBase () = CompSpodumain.Base
                override this.GetBase5 () = CompSpodumain.Base5
                override this.GetBase10 () = CompSpodumain.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompSpodumain.Yield
                override this.GetVolume () = CompSpodumain.Volume

            static member val Base   = 28420 with get
            static member val Base5  = 28418 with get
            static member val Base10 = 28419 with get
            static member val Yield  = CompressedYield Spodumain.Yield with get
            static member val Volume = 16.0f with get


        type CompCrokite () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Crokite"
                override this.GetName5 () =  "Compressed Sharp Crokite"
                override this.GetName10 () = "Compressed Crystalline Crokite"
                override this.GetBase () = CompCrokite.Base
                override this.GetBase5 () = CompCrokite.Base5
                override this.GetBase10 () = CompCrokite.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompCrokite.Yield
                override this.GetVolume () = CompCrokite.Volume

            static member val Base   = 28391 with get
            static member val Base5  = 28392 with get
            static member val Base10 = 28393 with get
            static member val Yield  = CompressedYield Crokite.Yield with get
            static member val Volume = 7.81f with get


        type CompBistot () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Bistot"
                override this.GetName5 () =  "Compressed Triclinic Bistot"
                override this.GetName10 () = "Compressed Monoclinic Bistot"
                override this.GetBase () = CompBistot.Base
                override this.GetBase5 () = CompBistot.Base5
                override this.GetBase10 () = CompBistot.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompBistot.Yield
                override this.GetVolume () = CompBistot.Volume

            static member val Base   = 28388 with get
            static member val Base5  = 28389 with get
            static member val Base10 = 28390 with get
            static member val Yield  = CompressedYield Bistot.Yield with get
            static member val Volume = 6.11f with get


        type CompArkonor () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Arkonor"
                override this.GetName5 () =  "Compressed Crimson Arkonor"
                override this.GetName10 () = "Compressed Prime Arkonor"
                override this.GetBase () = CompArkonor.Base
                override this.GetBase5 () = CompArkonor.Base5
                override this.GetBase10 () = CompArkonor.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompArkonor.Yield
                override this.GetVolume () = CompArkonor.Volume

            static member val Base   = 28367 with get
            static member val Base5  = 28385 with get
            static member val Base10 = 28387 with get
            static member val Yield  = CompressedYield Arkonor.Yield with get
            static member val Volume = 3.08f with get


        type CompMercoxit () =
            interface ICompressedOre with
                override this.GetName () =   "Compressed Mercoxit"
                override this.GetName5 () =  "Compressed Magma Mercoxit"
                override this.GetName10 () = "Compressed Vitreous Mercoxit"
                override this.GetBase () = CompMercoxit.Base
                override this.GetBase5 () = CompMercoxit.Base5
                override this.GetBase10 () = CompMercoxit.Base10
                override this.IsTiny () = false
                override this.GetYield ()  = CompMercoxit.Yield
                override this.GetVolume () = CompMercoxit.Volume

            static member val Base   = 28413 with get
            static member val Base5  = 28412 with get
            static member val Base10 = 28414 with get
            static member val Yield  = CompressedYield Mercoxit.Yield with get
            static member val Volume = 0.1f with get