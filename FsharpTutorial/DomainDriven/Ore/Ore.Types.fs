namespace EveOnline.OreDomain

module Types =
    type OreRarity = 
    | Common 
    | Uncommon
    | Rare


    type OreType = 
    | Arkonor     of int
    | Bistot      of int  
    | Crokite     of int  
    | DarkOchre   of int  
    | Gneiss      of int  
    | Hedbergite  of int  
    | Hemorphite  of int  
    | Jaspet      of int  
    | Kernite     of int  
    | Mercoxit    of int  
    | Omber       of int  
    | Plagioclase of int  
    | Pyroxeres   of int  
    | Scordite    of int  
    | Spodumain   of int  
    | Veldspar    of int  

    type Ore = 
    | CommonVeldspar    
    | UncommonVeldspar  
    | RareVeldspar      
    | CompressedCommonVeldspar   
    | CompressedUncommonVeldspar 
    | CompressedRareVeldspar     
    | CommonScordite    
    | UncommonScordite  
    | RareScordite      
    | CompressedCommonScordite    
    | CompressedUncommonScordite  
    | CompressedRareScordite      
    | CommonPyroxeres   
    | UncommonPyroxeres 
    | RarePyroxeres     
    | CompressedCommonPyroxeres   
    | CompressedUncommonPyroxeres 
    | CompressedRarePyroxeres     
    | CommonPlagioclase     
    | UncommonPlagioclase   
    | RarePlagioclase       
    | CompressedCommonPlagioclase    
    | CompressedUncommonPlagioclase  
    | CompressedRarePlagioclase      
    | CommonKernite         
    | UncommonKernite       
    | RareKernite           
    | CompressedCommonKernite   
    | CompressedUncommonKernite 
    | CompressedRareKernite     
    | CommonOmber           
    | UncommonOmber         
    | RareOmber             
    | CompressedCommonOmber   
    | CompressedUncommonOmber 
    | CompressedRareOmber     
    | CommonHedbergite      
    | UncommonHedbergite    
    | RareHedbergite        
    | CompressedCommonHedbergite   
    | CompressedUncommonHedbergite 
    | CompressedRareHedbergite     
    | CommonHemorphite      
    | UncommonHemorphite    
    | RareHemorphite        
    | CompressedCommonHemorphite   
    | CompressedUncommonHemorphite 
    | CompressedRareHemorphite     
    | CommonJaspet           
    | UncommonJaspet         
    | RareJaspet             
    | CompressedCommonJaspet    
    | CompressedUncommonJaspet  
    | CompressedRareJaspet      
    | CommonGneiss           
    | UncommonGneiss         
    | RareGneiss             
    | CompressedCommonGneiss   
    | CompressedUncommonGneiss 
    | CompressedRareGneiss     
    | CommonDarkOchre       
    | UncommonDarkOchre     
    | RareDarkOchre         
    | CompressedCommonDarkOchre   
    | CompressedUncommonDarkOchre 
    | CompressedRareDarkOchre     
    | CommonCrokite         
    | UncommonCrokite       
    | RareCrokite           
    | CompressedCommonCrokite  
    | CompressedUncommonCrokite
    | CompressedRareCrokite    
    | CommonBistot          
    | UncommonBistot        
    | RareBistot            
    | CompressedCommonBistot  
    | CompressedUncommonBistot
    | CompressedRareBistot  
    | CommonSpodumain       
    | UncommonSpodumain     
    | RareSpodumain         
    | CompressedCommonSpodumain      
    | CompressedUncommonSpodumain    
    | CompressedRareSpodumain   
    | CommonArkonor         
    | UncommonArkonor       
    | RareArkonor           
    | CompressedCommonArkonor   
    | CompressedUncommonArkonor 
    | CompressedRareArkonor     
    | CommonMercoxit        
    | UncommonMercoxit      
    | RareMercoxit          
    | CompressedCommonMercoxit  
    | CompressedUncommonMercoxit
    | CompressedRareMercoxit    

