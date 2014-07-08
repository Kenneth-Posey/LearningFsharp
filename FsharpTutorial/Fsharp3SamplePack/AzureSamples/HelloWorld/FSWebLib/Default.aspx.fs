// Learn more about F# at http://fsharp.net

namespace FSharp.Samples.HelloWorld
open System
open System.Web.UI.HtmlControls
open System.Web.UI.WebControls

type WebForm1() = 
    inherit System.Web.UI.Page()
   
    member public this.Page_Load(sender:obj, e:EventArgs) = 

        let addControlWithBreakLine (form1:HtmlForm) control =
            control |> form1.Controls.Add
            new Literal(Text="</br>") |> form1.Controls.Add

        let form1 = new HtmlForm()
        this.Controls.Add(form1)
        new Label(Text="Hello World!") |> addControlWithBreakLine form1
    
        new Label(Text="To get started creating applications for Windows Azure, see:") |> addControlWithBreakLine form1

        new HyperLink(Text="Windows Azure Hands On Labs",
                      NavigateUrl="http://msdn.microsoft.com/en-us/windowsazure/wazplatformtrainingcourse_windowsazure_unit")
                      |> addControlWithBreakLine form1

        new HyperLink(Text="Windows Azure Code Samples",
                      NavigateUrl="http://msdn.microsoft.com/en-us/library/windows-azure-code-samples.aspx")
                      |> addControlWithBreakLine form1

        new HyperLink(Text="Windows Azure Code Quick Start",
                      NavigateUrl="http://msdn.microsoft.com/en-us/library/gg663908.aspx")
                      |> addControlWithBreakLine form1