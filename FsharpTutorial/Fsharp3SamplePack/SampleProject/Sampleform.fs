// Copyright (c) Microsoft Corporation 2005-2011.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

module Display

open System
//open System.Collections.Generic
open System.ComponentModel
open System.Data
open System.Drawing 
open System.Text
open System.Windows.Forms
open System.IO
open Support.Helper
open Printf

let keywords = 
   [  "abstract";"and";"as";"assert"; "asr";
      "begin"; "class"; "default";
      "delegate"; "do";"done";
      "downcast";"downto"; "use";
      "elif";"else";"end";"exception";
      "false";"finally";"for";"fun";"function";
      "if";"in"; "inherit"; "inline";
      "interface"; "land"; "lazy"; "let";
      "lor"; "lsl";
      "lsr"; "lxor";
      "match"; "member";"mod";"module";
      "mutable";"namespace"; "new";"null"; "of";"object";"open";
      "or"; "override";
      "property";"rec";"static";"struct"; "sig";
      "then";"to";"true";"try";
      "type";"upcast"; "val";"when";
      "while";"with";"="; ":?"; "->"; "#light";
      "select";"where";"join";"groupJoin"  ]

let colorizeCode(rtb: # RichTextBox) = 
    let text = rtb.Text 
            
    rtb.SelectAll()
    rtb.SelectionColor <- rtb.ForeColor

    keywords |> List.iter (fun keyword -> 
        let mutable keywordPos = rtb.Find(keyword, RichTextBoxFinds.MatchCase ||| RichTextBoxFinds.WholeWord)
        while (keywordPos <> -1) do 
            let underscorePos = text.IndexOf("_", keywordPos)
            let commentPos = text.LastIndexOf("//", keywordPos)
            let newLinePos = text.LastIndexOf('\n', keywordPos)
            let mutable quoteCount = 0
            let mutable quotePos = text.IndexOf("\"", newLinePos + 1, keywordPos - newLinePos)
            while (quotePos <> -1) do
                quoteCount <- quoteCount + 1
                quotePos <- text.IndexOf("\"", quotePos + 1, keywordPos - (quotePos + 1))
            
            if (newLinePos >= commentPos && 
                underscorePos <> keywordPos + rtb.SelectionLength  && 
                quoteCount % 2 = 0) 
            then
                rtb.SelectionColor <- Color.Blue;

            keywordPos <- rtb.Find(keyword, keywordPos + rtb.SelectionLength, RichTextBoxFinds.MatchCase ||| RichTextBoxFinds.WholeWord)
    );
    rtb.Select(0, 0)

let getExecutionResult sampleOption = 
    use stream = new MemoryStream()  
    use writer = new StreamWriter(stream)  
    stream.SetLength(0L);
    match sampleOption with 
      | Some s -> 
        let oldConsoleOut = Console.Out  
        try 
          Console.SetOut(writer);
          try s.Run()
          with e -> 
            Console.WriteLine("Exception raised: {0}",e)
        finally Console.SetOut(oldConsoleOut)
      | None -> ()
    writer.Flush();
    writer.Encoding.GetString(stream.ToArray());

type SampleForm(title, harnesses) as this  = 
    inherit Form()
    let mutable currentSample : sample option = None

    let mutable components: System.ComponentModel.Container = null

    let mutable outerSplitContainer : System.Windows.Forms.SplitContainer = null
    let mutable samplesLabel : System.Windows.Forms.Label = null
    let mutable rightContainer : System.Windows.Forms.SplitContainer = null
    let mutable outputTextBox : System.Windows.Forms.RichTextBox = null
    let mutable outputLabel : System.Windows.Forms.Label = null
    let mutable runButton : System.Windows.Forms.Button = null
    let mutable rightUpperSplitContainer : System.Windows.Forms.SplitContainer = null
    let mutable descriptionTextBox : System.Windows.Forms.TextBox = null
    let mutable descriptionLabel : System.Windows.Forms.Label = null
    let mutable codeLabel : System.Windows.Forms.Label = null
    let mutable samplesTreeView : System.Windows.Forms.TreeView = null
    let mutable imageList : System.Windows.Forms.ImageList = null
    let mutable codeRichTextBox : System.Windows.Forms.RichTextBox = null
    do
        this.InitializeComponent();

        this.Text <- title;

        let rootNode = new TreeNode(title, Tag = (null : obj), ImageKey = "Help", SelectedImageKey = "Help")
        ignore(samplesTreeView.Nodes.Add(rootNode));
        rootNode.Expand();

        harnesses |> List.iter (fun (title,samples) -> 
            let harnessNode = new TreeNode(title)  
            harnessNode.Tag <- (null : obj);
            harnessNode.ImageKey <- "BookStack";
            harnessNode.SelectedImageKey <- "BookStack";
            ignore(rootNode.Nodes.Add(harnessNode));

            let category = ref ""  
            let categoryNode = ref (null: TreeNode)  
            samples |> List.iter (fun (sample: sample) ->
                if (sample.Category <> !category) then 
                    let n = new TreeNode(sample.Category) 
                    n.Tag <- (null : obj);
                    n.ImageKey <- "BookClosed";
                    n.SelectedImageKey <- "BookClosed";
                    ignore(harnessNode.Nodes.Add(n));
                    category := sample.Category;
                    categoryNode := n;
               
                let node = new TreeNode(sample.Title)  
                node.Tag <- sample;
                node.ImageKey <- "Item";
                node.SelectedImageKey <- "Item";
                ignore((!categoryNode).Nodes.Add(node))
              )
        )
    override this.Dispose(disposing) =
        if (disposing && (match components with null -> false | _ -> true)) then
          components.Dispose();
        base.Dispose(disposing)

    member this.InitializeComponent() =
        components <- new System.ComponentModel.Container();
        let resources = new System.ComponentModel.ComponentResourceManager(typeof<SampleForm>) 
        outerSplitContainer <- new System.Windows.Forms.SplitContainer();
        samplesTreeView <- new System.Windows.Forms.TreeView();
        imageList <- new System.Windows.Forms.ImageList(components);
        samplesLabel <- new System.Windows.Forms.Label();
        rightContainer <- new System.Windows.Forms.SplitContainer();
        rightUpperSplitContainer <- new System.Windows.Forms.SplitContainer();
        descriptionTextBox <- new System.Windows.Forms.TextBox();
        descriptionLabel <- new System.Windows.Forms.Label();
        codeRichTextBox <- new System.Windows.Forms.RichTextBox();
        codeLabel <- new System.Windows.Forms.Label();
        runButton <- new System.Windows.Forms.Button();
        outputTextBox <- new System.Windows.Forms.RichTextBox();
        outputLabel <- new System.Windows.Forms.Label();
        outerSplitContainer.Panel1.SuspendLayout();
        outerSplitContainer.Panel2.SuspendLayout();
        outerSplitContainer.SuspendLayout();
        rightContainer.Panel1.SuspendLayout();
        rightContainer.Panel2.SuspendLayout();
        rightContainer.SuspendLayout();
        rightUpperSplitContainer.Panel1.SuspendLayout();
        rightUpperSplitContainer.Panel2.SuspendLayout();
        rightUpperSplitContainer.SuspendLayout();
        this.SuspendLayout();
        // 
        // outerSplitContainer
        // 
        outerSplitContainer.Dock <- System.Windows.Forms.DockStyle.Fill;
        outerSplitContainer.FixedPanel <- System.Windows.Forms.FixedPanel.Panel1;
        outerSplitContainer.Location <- new System.Drawing.Point(0, 0);
        outerSplitContainer.Name <- "outerSplitContainer";
        // 
        // outerSplitContainer.Panel1
        // 
        outerSplitContainer.Panel1.Controls.Add(samplesTreeView);
        outerSplitContainer.Panel1.Controls.Add(samplesLabel);
        // 
        // outerSplitContainer.Panel2
        // 
        outerSplitContainer.Panel2.Controls.Add(rightContainer);
        outerSplitContainer.Size <- new System.Drawing.Size(952, 682);
        outerSplitContainer.SplitterDistance <- 268;
        outerSplitContainer.TabIndex <- 0;
        // 
        // samplesTreeView
        // 
        samplesTreeView.Anchor <- 
          System.Windows.Forms.AnchorStyles.Top ||| 
          System.Windows.Forms.AnchorStyles.Bottom |||
          System.Windows.Forms.AnchorStyles.Left |||
          System.Windows.Forms.AnchorStyles.Right;
        samplesTreeView.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        samplesTreeView.HideSelection <- false;
        samplesTreeView.ImageKey <- "Item";
        samplesTreeView.ImageList <- imageList;
        samplesTreeView.Location <- new System.Drawing.Point(0, 28);
        samplesTreeView.Name <- "samplesTreeView";
        samplesTreeView.SelectedImageKey <- "Item";
        samplesTreeView.ShowNodeToolTips <- true;
        samplesTreeView.ShowRootLines <- false;
        samplesTreeView.Size <- new System.Drawing.Size(266, 654);
        samplesTreeView.TabIndex <- 1;
        samplesTreeView.add_AfterExpand(fun _ e -> 
              match e.Node.Level with 
              | 1 | 2 -> 
                e.Node.ImageKey <- "BookOpen";
                e.Node.SelectedImageKey <- "BookOpen";
              | _ -> ());
          
          
        samplesTreeView.BeforeCollapse.Add(fun e -> 
              match e.Node.Level with 
              | 0 -> 
                e.Cancel <- true;
              | _ -> ());
            
        samplesTreeView.AfterSelect.Add(fun e -> 
            let currentNode = samplesTreeView.SelectedNode  
            match currentNode.Tag with 
            | null -> 
                runButton.Enabled <- false;
                descriptionTextBox.Text <- Support.Helper.SampleAttributes.getResourceString ("SelectLeft", this.GetType())
                codeRichTextBox.Clear();
                outputTextBox.Clear();
                if (e.Action <> TreeViewAction.Collapse && e.Action <> TreeViewAction.Unknown) then
                    e.Node.Expand();
            | :?  sample as s -> 
                currentSample <- Some s;
                runButton.Enabled <- true;
                descriptionTextBox.Text <- s.Description;
                codeRichTextBox.Clear();
                codeRichTextBox.Text <- s.Code;
                colorizeCode(codeRichTextBox);
                outputTextBox.Clear()
            | _ -> ());
              
        samplesTreeView.AfterCollapse.Add(fun e -> 
          match e.Node.Level with 
          | 1 -> 
            e.Node.ImageKey <- "BookStack";
            e.Node.SelectedImageKey <- "BookStack";
          |  2 ->
            e.Node.ImageKey <- "BookClosed";
            e.Node.SelectedImageKey <- "BookClosed"
          | _ -> ());

        // 
        // imageList
        // 
        imageList.ImageStream <- (resources.GetObject("imageList.ImageStream") :?> System.Windows.Forms.ImageListStreamer);
        imageList.Images.SetKeyName(0, "Help");
        imageList.Images.SetKeyName(1, "BookStack");
        imageList.Images.SetKeyName(2, "BookClosed");
        imageList.Images.SetKeyName(3, "BookOpen");
        imageList.Images.SetKeyName(4, "Item");
        imageList.Images.SetKeyName(5, "Run");
        // 
        // samplesLabel
        // 
        samplesLabel.AutoSize <- true;
        samplesLabel.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        samplesLabel.Location <- new System.Drawing.Point(3, 9);
        samplesLabel.Name <- "samplesLabel";
        samplesLabel.Size <- new System.Drawing.Size(58, 16);
        samplesLabel.TabIndex <- 0;
        samplesLabel.Text <- Support.Helper.SampleAttributes.getResourceString ("SampleTag", this.GetType());
        // 
        // rightContainer
        // 
        rightContainer.Dock <- System.Windows.Forms.DockStyle.Fill;
        rightContainer.Location <- new System.Drawing.Point(0, 0);
        rightContainer.Name <- "rightContainer";
        rightContainer.Orientation <- System.Windows.Forms.Orientation.Horizontal;
        // 
        // rightContainer.Panel1
        // 
        rightContainer.Panel1.Controls.Add(rightUpperSplitContainer);
        // 
        // rightContainer.Panel2
        // 
        rightContainer.Panel2.Controls.Add(runButton);
        rightContainer.Panel2.Controls.Add(outputTextBox);
        rightContainer.Panel2.Controls.Add(outputLabel);
        rightContainer.Size <- new System.Drawing.Size(680, 682);
        rightContainer.SplitterDistance <- 357;
        rightContainer.TabIndex <- 0;
        // 
        // rightUpperSplitContainer
        // 
        rightUpperSplitContainer.Dock <- System.Windows.Forms.DockStyle.Fill;
        rightUpperSplitContainer.FixedPanel <- System.Windows.Forms.FixedPanel.Panel1;
        rightUpperSplitContainer.Location <- new System.Drawing.Point(0, 0);
        rightUpperSplitContainer.Name <- "rightUpperSplitContainer";
        rightUpperSplitContainer.Orientation <- System.Windows.Forms.Orientation.Horizontal;
        // 
        // rightUpperSplitContainer.Panel1
        // 
        rightUpperSplitContainer.Panel1.Controls.Add(descriptionTextBox);
        rightUpperSplitContainer.Panel1.Controls.Add(descriptionLabel);
        // 
        // rightUpperSplitContainer.Panel2
        // 
        rightUpperSplitContainer.Panel2.Controls.Add(codeRichTextBox);
        rightUpperSplitContainer.Panel2.Controls.Add(codeLabel);
        rightUpperSplitContainer.Size <- new System.Drawing.Size(680, 357);
        rightUpperSplitContainer.SplitterDistance <- 95;
        rightUpperSplitContainer.TabIndex <- 0;
        // 
        // descriptionTextBox
        // 
        descriptionTextBox.Anchor<- 
          System.Windows.Forms.AnchorStyles.Top |||
          System.Windows.Forms.AnchorStyles.Bottom |||
          System.Windows.Forms.AnchorStyles.Left |||
          System.Windows.Forms.AnchorStyles.Right;
        descriptionTextBox.BackColor <- System.Drawing.SystemColors.ControlLight;
        descriptionTextBox.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        descriptionTextBox.Location <- new System.Drawing.Point(0, 28);
        descriptionTextBox.Multiline <- true;
        descriptionTextBox.Name <- "descriptionTextBox";
        descriptionTextBox.ReadOnly <- true;
        descriptionTextBox.ScrollBars <- System.Windows.Forms.ScrollBars.Vertical;
        descriptionTextBox.Size <- new System.Drawing.Size(680, 67);
        descriptionTextBox.TabIndex <- 1;
        // 
        // descriptionLabel
        // 
        descriptionLabel.AutoSize <- true;
        descriptionLabel.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        descriptionLabel.Location <- new System.Drawing.Point(3, 9);
        descriptionLabel.Name <- "descriptionLabel";
        descriptionLabel.Size <- new System.Drawing.Size(72, 16);
        descriptionLabel.TabIndex <- 0;
        descriptionLabel.Text <- Support.Helper.SampleAttributes.getResourceString ("DescriptionTag", this.GetType())
        // 
        // codeRichTextBox
        // 
        codeRichTextBox.Anchor <- 
          System.Windows.Forms.AnchorStyles.Top |||
          System.Windows.Forms.AnchorStyles.Bottom |||
          System.Windows.Forms.AnchorStyles.Left |||
          System.Windows.Forms.AnchorStyles.Right;
        codeRichTextBox.BackColor <- System.Drawing.SystemColors.ControlLight;
        //codeRichTextBox.ForeColor <- System.Drawing.Color.Blue;
        codeRichTextBox.BorderStyle <- System.Windows.Forms.BorderStyle.FixedSingle;
        codeRichTextBox.Font <- new System.Drawing.Font("Lucida Console", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        codeRichTextBox.Location <- new System.Drawing.Point(0, 18);
        codeRichTextBox.Name <- "codeRichTextBox";
        codeRichTextBox.ReadOnly <- true;
        codeRichTextBox.Size <- new System.Drawing.Size(680, 240);
        codeRichTextBox.TabIndex <- 1;
        codeRichTextBox.Text <- "";
        //codeRichTextBox.Dock <- DockStyle.Fill;
        codeRichTextBox.WordWrap <- false;
        // 
        // codeLabel
        // 
        codeLabel.AutoSize <- true;
        codeLabel.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        codeLabel.Location <- new System.Drawing.Point(3, -1);
        codeLabel.Name <- "codeLabel";
        codeLabel.Size <- new System.Drawing.Size(38, 16);
        codeLabel.TabIndex <- 0;
        codeLabel.Text <- Support.Helper.SampleAttributes.getResourceString ("CodeTag", this.GetType());
        // 
        // runButton
        // 
        runButton.Enabled <- false;
        runButton.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        runButton.ImageAlign <- System.Drawing.ContentAlignment.MiddleRight;
        runButton.ImageKey <- "Run";
        runButton.ImageList <- imageList;
        runButton.Location <- new System.Drawing.Point(0, -1);
        runButton.Name <- "runButton";
        runButton.Size <- new System.Drawing.Size(119, 27);
        runButton.TabIndex <- 0;
        runButton.Text <- Support.Helper.SampleAttributes.getResourceString ("RunSample", this.GetType())
        runButton.TextImageRelation <- System.Windows.Forms.TextImageRelation.ImageBeforeText;
        runButton.Click.Add(fun e -> 
         
              this.UseWaitCursor <- true;
              try 
                outputTextBox.Text <- "";
                let executionResult = getExecutionResult currentSample
                outputTextBox.Text <- outputTextBox.Text + executionResult;
              finally
                this.UseWaitCursor <- false);
            // 
        // outputTextBox
        // 
        outputTextBox.Anchor <- 
          System.Windows.Forms.AnchorStyles.Top |||
          System.Windows.Forms.AnchorStyles.Bottom |||
          System.Windows.Forms.AnchorStyles.Left |||
          System.Windows.Forms.AnchorStyles.Right;
        outputTextBox.BackColor <- System.Drawing.SystemColors.ControlLight;
        outputTextBox.Font <- new System.Drawing.Font("Lucida Console", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        outputTextBox.Location <- new System.Drawing.Point(0, 48);
        outputTextBox.Multiline <- true;
        outputTextBox.Name <- "outputTextBox";
        outputTextBox.ReadOnly <- true;
        outputTextBox.ScrollBars <- System.Windows.Forms.RichTextBoxScrollBars.Both;
        outputTextBox.Size <- new System.Drawing.Size(680, 273);
        outputTextBox.TabIndex <- 2;
        outputTextBox.WordWrap <- false;
        // 
        // outputLabel
        // 
        outputLabel.AutoSize <- true;
        outputLabel.Font <- new System.Drawing.Font("Tahoma", 10.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0uy);
        outputLabel.Location <- new System.Drawing.Point(3, 29);
        outputLabel.Name <- "outputLabel";
        outputLabel.Size <- new System.Drawing.Size(47, 16);
        outputLabel.TabIndex <- 1;
        outputLabel.Text <- Support.Helper.SampleAttributes.getResourceString ("OutputTag", this.GetType());
        // 
        // DisplayForm
        // 
        this.AcceptButton <- runButton;
        this.AutoScaleDimensions <- new System.Drawing.SizeF(6.0F, 13.0F);
        this.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize <- new System.Drawing.Size(952, 682);
        this.Controls.Add(outerSplitContainer);
        this.Font <- new System.Drawing.Font("Tahoma", 8.25F);
        this.Icon <- (resources.GetObject("$this.Icon") :?> System.Drawing.Icon);
        this.Name <- "DisplayForm";
        this.Text <- Support.Helper.SampleAttributes.getResourceString ("SamplesTag", this.GetType());
        outerSplitContainer.Panel1.ResumeLayout(false);
        outerSplitContainer.Panel1.PerformLayout();
        outerSplitContainer.Panel2.ResumeLayout(false);
        outerSplitContainer.ResumeLayout(false);
        rightContainer.Panel1.ResumeLayout(false);
        rightContainer.Panel2.ResumeLayout(false);
        rightContainer.Panel2.PerformLayout();
        rightContainer.ResumeLayout(false);
        rightUpperSplitContainer.Panel1.ResumeLayout(false);
        rightUpperSplitContainer.Panel1.PerformLayout();
        rightUpperSplitContainer.Panel2.ResumeLayout(false);
        rightUpperSplitContainer.Panel2.PerformLayout();
        rightUpperSplitContainer.ResumeLayout(false);
        this.ResumeLayout(false)


  