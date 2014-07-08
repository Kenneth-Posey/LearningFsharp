
#r @"System.Management.dll"
#r @"..\..\Debug\net45\Samples.Management.TypeProvider.dll"

open System
open System.Management
open Samples.Management.TypeProvider 

// Access the default WMI namespace (root/cimv2) on the local machine and create a data context
// Note, use the 'Namespace' to access an alternative WMI namespace.
//
// type LocalWmi = WmiProvider<"localhost","root/wmi">
// let wmiData = LocalWmi.GetDataContext()

type Local = WmiProvider<"localhost">
let data = Local.GetDataContext()

// Add a handler to watch WMI queries getting executed
data.QueryExecuted.Add(printfn "Query executed: %s")

// Access some WMI data from the data connection
[for dd in data.CIM_DiskDrive -> [for c in dd.Capabilities -> c.Is_SMART_Notification]]

// Access a WMI event...
data.Win32_Process.InstanceCreationEvent.AddWithin(1).Add(fun p -> printfn "%s" p.TargetInstance.ExecutablePath)

// Add XML docs to builder types, instances, and methods (Within, etc.)
// Add XML docs to instance creation properties, etc.
let processCreationWithin1Second = 
    wmiEvent { for e in data.Win32_Process.InstanceCreationEvent do
               within 1
               where (e.TargetInstance.Name = "notepad.exe")
               select (e.TargetInstance.ExecutablePath, e.TargetInstance.CommandLine) }

// Get the query string associated with a WMI event (e.g. for diagnostics)
let qs = processCreationWithin1Second.WmiQueryString

// Subscribe to an a WMI event
let subscription = processCreationWithin1Second.Subscribe(fun eventArgs -> printfn "%A" eventArgs)
subscription.Dispose()

// Test static WMI method, with a unsigned int32 out param
let resultCode1, recCount = data.Win32_ReliabilityRecords.GetRecordCount()

// Test a static WMI method, with an unsigned int32 out param
let resultCode2, processId = data.Win32_Process.Create("notepad.exe", null, null)

// Test conversion of datetime properties
[ for p in data.Win32_Process -> (p.Name, p.PageFaults, p.CreationDate.ToString()) ]

type System.Collections.Generic.IEnumerable<'T> with 
    member xs.Head = Seq.head xs


// Test enum array properties
let drive = data.CIM_DiskDrive.Head

[for c in drive.Capabilities -> c.Is_Random_Access]
Local.ServiceTypes.CIM_DiskDrive.CapabilitiesValues.``Automatic Cleaning``

// test embedded object properties (n.b. 'Documents' is Windows 8 only)
// (data.Win32_UserProfile |> Seq.head).Documents.OfflineFileNameFolderGUID

// test reference properties
data.Win32_SystemBIOS.Head.GroupComponent.Value.AdminPasswordStatus

// test intrinsic events
let ev = 
    wmiEvent { for e in data.Win32_Process.InstanceCreationEvent do
               within 5
               where (e.TargetInstance.Name = "notepad.exe" || e.TargetInstance.Name = "ildasm.exe")
               select e.TargetInstance.ProcessId.Value }

let sub = ev.Subscribe(fun pid -> printfn "%i" pid)
sub.Dispose()

// test extrinsic events
// Note: WMI is picky; all three query conditions are required or you'll get an inscrutable error
let ev2 = 
    wmiEvent { for e in data.Win32_DeviceChangeEvent do
               select e }

let sub2 = ev2.Subscribe(fun e -> printfn "%A" e.EventType)
// use device manager to disable/enable devices

sub2.Dispose()

// test queries
let files = wmiQuery {
    for f in data.CIM_DataFile do
    where (f.Drive = "c:" && f.Path = @"\users\a-kebatt\") }

// test instance methods
files.Head.Copy(@"C:\Users\a-kebatt\Documents\test")

// test more elaborate conditions in queries
open Linq.NullableOperators

// Nullables are supported (either with (?=), etc., or via .Value and .HasValue)
wmiQuery { for dd in data.Win32_DiskDrive do
           where (dd.NeedsCleaning ?= true) }

wmiQuery { for dd in data.Win32_DiskDrive do
           where (not dd.NeedsCleaning.Value) }

wmiQuery { for dd in data.Win32_DiskDrive do
           where (not dd.NeedsCleaning.HasValue) }

let qs2 = 
    (wmiQuery {
        for f in data.CIM_DataFile do
        where (f.FileSize ?> 100000000uL)
    }).WmiQueryString

[for c in data.Win32_ComputerSystem -> c.Manufacturer, c.Model, c.TotalPhysicalMemory.GetValueOrDefault() / (pown 2uL 30)]
[for c in data.Win32_OperatingSystem -> c.Manufacturer]

open Linq.NullableOperators
wmiQuery { for f in data.CIM_DataFile do
           where (f.Path = @"\Windows\System")
           where (f.Drive = "C")
           where (f.LastModified ?>= DateTimeOffset.Parse("1/1/2012")) }

[ for b in data.Win32_DiskDrive -> b.Name, b.Description]

[ for b in data.CIM_Battery -> b.Name, b.BatteryStatus, b.Availability, b.Description, b.ExpectedLife, b.TimeOnBattery, b.SmartBatteryVersion ] 

[ for b in data.Win32_NetworkAdapter -> b.Description, b.InstallDate ] 


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
// this takes a long time - it is polling the domain controller for all accounts
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


let data3 = Local.GetDataContext("MSRC-3765408")
    
[ for x in data3.Win32_PointingDevice -> x ]

#if ADMIN

// Note: need to run as admin to get drive health, or you'll get an "Access denied" error
[for status in wmiData.MSStorageDriver_FailurePredictStatus -> status.Reason]
#endif

// This downloads the remote schema 
#if REMOTE
type RemoteSchema = WmiProvider<MachineName="MSRC-3765408">
let data2 = RemoteSchema.GetDataContext()

for bios in data2.Win32_BIOS do
    printfn "bios versions: '%A'" bios.BIOSVersion

// This checks the parsing of DMT dates into System.DateTime
[ for p in data2.Win32_Process -> (p.Name, p.PageFaults, p.CreationDate.ToString()) ]
#endif

