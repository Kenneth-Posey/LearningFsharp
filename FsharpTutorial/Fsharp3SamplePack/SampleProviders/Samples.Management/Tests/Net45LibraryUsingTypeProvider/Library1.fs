module LibraryUsingTypeProvider

open System
open System.Management
open Samples.Management.TypeProvider 

type Local = WmiProvider<"localhost">
let data = Local.GetDataContext()

[ for b in data.Win32_DiskDrive -> b.Name, b.Description]

[ for b in data.CIM_Battery -> b.Name, b.BatteryStatus, b.Availability, b.Description, b.ExpectedLife, b.TimeOnBattery, b.SmartBatteryVersion ] 

// Display the computer information
for m in data.Win32_OperatingSystem do
    Console.WriteLine("Computer Name : {0}", m.Name)
    Console.WriteLine("Description : {0}", m.Description)
    Console.WriteLine("BootDevice : {0}", m.BootDevice)
    Console.WriteLine("Windows Directory : {0}", m.WindowsDirectory)
    Console.WriteLine("Operating System: {0}",  m.Caption)
    Console.WriteLine("Version: {0}", m.Version)
    Console.WriteLine("Manufacturer : {0}", m.Manufacturer)

for proc in data.Win32_Process do
    printfn "process name is '%s', peak working set size is '%A'" proc.Name proc.PeakWorkingSetSize

for bios in data.Win32_BIOS do
    printfn "bios versions: '%A'" bios.BIOSVersion

data.MSFT_NCProvEvent
data.RegistryEvent
[ for x in data.CIM_BIOSElement -> x.Name ]
[ for x in data.CIM_BIOSFeature -> x.Name ]
[ for x in data.CIM_VideoBIOSElement -> x.Name ]
[ for x in data.Win32_Keyboard -> x.Name ]
[ for x in data.Win32_SoundDevice -> x.Name ]
[ for x in data.Win32_COMApplication -> x.Name ]
[ for x in data.Win32_COMApplication -> x.Caption ]
[ for x in data.Win32_COMApplication -> x.Description ]
[ for x in data.Win32_COMApplication -> x.InstallDate ]
[ for x in data.Win32_COMApplication -> x.Status ]
[ for x in data.Win32_Environment -> x.UserName ]
[ for x in data.Win32_Environment -> x.Name ]
[ for x in data.Win32_Environment -> x.Description ]
[ for x in data.Win32_Environment -> x.VariableValue ]
[ for x in data.Win32_Environment -> x.SystemVariable ]
[ for x in data.Win32_Account -> x.Name ]
[ for x in data.Win32_Account -> x.LocalAccount ]
[ for x in data.Win32_Account -> x.SID ]
[ for x in data.Win32_Account -> x.SIDType ]
Local.ServiceTypes.Win32_Account.SIDTypeValues.SidTypeAlias
Local.ServiceTypes.Win32_Account.SIDTypeValues.SidTypeWellKnownGroup
Local.ServiceTypes.Win32_Account.SIDTypeValues.SidTypeAlias.Is_SidTypeAlias
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeGroup ]
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeUser ]
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeAlias ]
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeUnknown ]
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeComputer ]
[ for x in data.Win32_Account -> x.SIDType.Is_SidTypeWellKnownGroup ]
[ for x in data.Win32_Account -> x.SIDType ]
[ for x in data.Win32_SystemAccount -> x.Name ]
[ for x in data.Win32_UserAccount -> x.Name ]
[ for x in data.Win32_PointingDevice -> x ]


// test static method, uint out param
data.Win32_Process.Create("notepad.exe", null, null)

// test static method, uint out param
data.Win32_Process.Create("notepad.exe", null, null)

// test conversion of datetime properties
[ for p in data.Win32_Process -> (p.Name, p.PageFaults, p.CreationDate.ToString()) ]

let ld = data.Win32_LogicalDisk |> Seq.head

let access = ld.Access

match ld.Access with 
| null -> 0
| a when a.Is_Unknown -> 1
| a when a.Is_Writeable -> 2 
| a when a.Is_Write_Once -> 3
| _ -> 4

