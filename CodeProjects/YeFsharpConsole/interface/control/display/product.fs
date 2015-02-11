namespace WinformsPractice

module Product = 
    open System.Windows.Forms

    let label text position = 
        (new Label ())
        |> fun x -> x.Text <- text
                    x.Location <- position
                    x

    let button text onclick position = 
        (new Button ())
        |> fun x -> x.Text <- text
                    x.Location <- position
                    x.Click.Add onclick
                    x

    let textbox text position = 
        (new TextBox ())
        |> fun x -> x.Text <- text
                    x.Location <- position
                    x

    let combobox items position = 
        (new ComboBox ())
        |> fun x -> x.DataSource <- items
                    x.Location <- position
                    x

    let checkbox ischecked position = 
        (new CheckBox ())
        |> fun x -> x.Checked <- ischecked
                    x.Location <- position
                    x