using STFN.Core;

public class SelectionButtonSet : Grid
{

    public readonly Orientation orientation = Orientation.Horizontal;
    public readonly int setOrder = 0;

    public enum Orientation {
        Vertical,
        Horizontal
    }

    public event SelectionTextButtonEventHandler NewSelection;

    public SelectionButtonSet(List<SpeechTestResponseAlternative> responseAlternatives, double textSize, Orientation orientation, int setOrder)
     : this(responseAlternatives, textSize, orientation, setOrder, Color.FromRgb(4, 255, 61), Color.FromRgb(40, 40, 40)) { }

    public SelectionButtonSet(List<SpeechTestResponseAlternative> responseAlternatives, double textSize, Orientation orientation, int setOrder, Color selectionColor, Color unSelectedColor)
    {

        this.orientation = orientation;
        this.setOrder = setOrder;

        int nItems = responseAlternatives.Count;
        int nRows;
        int nCols;

        //this.BackgroundColor = Colors.Aquamarine;

        if (orientation ==  Orientation.Horizontal) {
            nRows = 1;
            nCols = nItems;
        }
        else
        {
            nRows = nItems;
            nCols = 1;
        }

        // Setting up rows and columns
        for (int i = 0; i < nRows; i++)
        {
            this.AddRowDefinition(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < nCols; i++)
        {
            this.AddColumnDefinition(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Adding SelectionButtonSets
        if (orientation == Orientation.Horizontal)
        {
            // As a row
            for (int i = 0; i < responseAlternatives.Count; i++)
            {
                SelectionTextButton selectionTextButton = new SelectionTextButton(responseAlternatives[i], textSize, selectionColor, unSelectedColor);
                selectionTextButton.Clicked += selectionButton_Clicked;
                this.Add(selectionTextButton, i, 1);
            }
        }
        else
        {
            // As a column
            for (int i = 0; i < responseAlternatives.Count; i++)
            {
                SelectionTextButton selectionTextButton = new SelectionTextButton(responseAlternatives[i], textSize, selectionColor, unSelectedColor);
                selectionTextButton.Clicked += selectionButton_Clicked;
                this.Add(selectionTextButton, 1, i);
            }
        }
    }

    private void selectionButton_Clicked(object sender, EventArgs e)
    {

        if (sender is Button) {

                Button senderButton = sender as Button;
                string NewSelectedText = "";

                foreach (SelectionTextButton selectionTextButton in this.Children)
                {
                    if (selectionTextButton.CheckButtonReferenceEquality(senderButton))
                    {
                        NewSelectedText = selectionTextButton.GetValue();
                    }
                    else
                    {
                        selectionTextButton.ShowAsSelected = false;
                    }
                }

                //Raising the clicked event of the SelectionTextButton (which can be listened to by an external class
                NewSelection?.Invoke(new SelectionTextButtonEventArgs(NewSelectedText, this.setOrder));

            }
    }

    public void TurnRed()
    {
        foreach (SelectionTextButton selectionTextButton in this.Children)
        {
            selectionTextButton.TurnRed();
        }
    }


    public class SelectionTextButtonEventArgs : EventArgs
    {

        private string selectedText;
        private int setOrder;
        public string SelectedText
        {
            get { return this.selectedText; }
        }
        public int SetOrder
        {
            get { return this.setOrder; }
        }

        public SelectionTextButtonEventArgs(string selectedText, int setOrder)
        {
            this.selectedText = selectedText;            
            this.setOrder = setOrder;
        }
    }

    public delegate void SelectionTextButtonEventHandler(SelectionTextButtonEventArgs e);

}





