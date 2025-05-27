namespace STFM.SpecializedViews.SSQ12;

public partial class SSQ12_IntroView : ContentView
{
    public SSQ12_IntroView()
    {
        InitializeComponent();

        for (int i = 1; i < 13; i++)
        {
            HA_Left_WeeksPicker.Items.Add(i.ToString());
            HA_Right_WeeksPicker.Items.Add(i.ToString());
        }

        for (int i = 0; i < 51; i++)
        {
            HA_Left_YearsPicker.Items.Add(i.ToString());
            HA_Right_YearsPicker.Items.Add(i.ToString());
        }

        for (int i = 0; i < 13; i++)
        {
            HA_Left_MonthsPicker.Items.Add(i.ToString());
            HA_Right_MonthsPicker.Items.Add(i.ToString());
        }

        HA_Left_WeeksPicker.IsVisible = false;
        HA_Right_WeeksPicker.IsVisible = false;
        HA_Left_YearsPicker.IsVisible = false;
        HA_Right_YearsPicker.IsVisible = false;
        HA_Left_MonthsPicker.IsVisible = false;
        HA_Right_MonthsPicker.IsVisible = false;

    }


    private void HA_QuestionRadioButton1_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryStackLayout.IsVisible = false;
    }

    private void HA_QuestionRadioButton2_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryStackLayout.IsVisible = true;
        HA_RightFrame.IsVisible = false;
        HA_LeftFrame.IsVisible = true;
        HA_Left_YearsMonth_RadioButton.IsChecked = true;

    }

    private void HA_QuestionRadioButton3_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryStackLayout.IsVisible = true;
        HA_RightFrame.IsVisible = true;
        HA_LeftFrame.IsVisible = false;
        HA_Right_YearsMonth_RadioButton.IsChecked = true;

    }

    private void HA_QuestionRadioButton4_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        HaHistoryStackLayout.IsVisible = true;
        HA_RightFrame.IsVisible = true;
        HA_LeftFrame.IsVisible = true;

        HA_Left_YearsMonth_RadioButton.IsChecked = true;
        HA_Right_YearsMonth_RadioButton.IsChecked = true;
    }


    public bool HasResponse()
    {

        if (HA_QuestionRadioButton1.IsChecked | HA_QuestionRadioButton2.IsChecked | HA_QuestionRadioButton3.IsChecked | HA_QuestionRadioButton4.IsChecked)
        {

            if (HA_QuestionRadioButton2.IsChecked | HA_QuestionRadioButton4.IsChecked)
            {
                if (HA_Left_YearsMonth_RadioButton.IsChecked)
                {
                    if (HA_Left_YearsPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet år du använt hörapparat på vänster öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet år!");
                        return false;
                    }
                    if (HA_Left_MonthsPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet månader du använt hörapparat på vänster öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet månader!");
                        return false;
                    }
                }
                else
                {
                    if (HA_Left_WeeksPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet veckor du använt hörapparat på vänster öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet veckor!");
                        return false;
                    }
                }
            }

            if (HA_QuestionRadioButton3.IsChecked | HA_QuestionRadioButton4.IsChecked)
            {
                if (HA_Right_YearsMonth_RadioButton.IsChecked)
                {
                    if (HA_Right_YearsPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet år du använt hörapparat på höger öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet år!");
                        return false;
                    }
                    if (HA_Right_MonthsPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet månader du använt hörapparat på höger öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet månader!");
                        return false;
                    }
                }
                else
                {
                    if (HA_Right_WeeksPicker.SelectedItem == null)
                    {
                        STFN.Messager.MsgBox("Vänligen ange antalet veckor du använt hörapparat på höger öra.", STFN.Messager.MsgBoxStyle.Information, "Ange antalet veckor!");
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            STFN.Messager.MsgBox("Vänligen besvara frågan innan du går vidare!", STFN.Messager.MsgBoxStyle.Information, "Frågan är inte besvarad!");
            return false;
        }

    }

    private void HA_Right_YearsMonth_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

        HA_Right_WeeksPicker.IsVisible = false;
        HA_Right_WeeksLabel.IsVisible = false;

        HA_Right_YearsPicker.IsVisible = true;
        HA_Right_YearsLabel.IsVisible = true;

        HA_Right_MonthsPicker.IsVisible = true;
        HA_Right_MonthsLabel.IsVisible = true;

    }

    private void HA_Right_Weeks_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

        HA_Right_WeeksPicker.IsVisible = true;
        HA_Right_WeeksLabel.IsVisible = true;

        HA_Right_YearsPicker.IsVisible = false;
        HA_Right_YearsLabel.IsVisible = false;

        HA_Right_MonthsPicker.IsVisible = false;
        HA_Right_MonthsLabel.IsVisible = false;

    }

    private void HA_Left_YearsMonth_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

        HA_Left_WeeksPicker.IsVisible = false;
        HA_Left_WeeksLabel.IsVisible = false;

        HA_Left_YearsPicker.IsVisible = true;
        HA_Left_YearsLabel.IsVisible = true;

        HA_Left_MonthsPicker.IsVisible = true;
        HA_Left_MonthsLabel.IsVisible = true;

    }

    private void HA_Left_Weeks_RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {

        HA_Left_WeeksPicker.IsVisible = true;
        HA_Left_WeeksLabel.IsVisible = true;

        HA_Left_YearsPicker.IsVisible = false;
        HA_Left_YearsLabel.IsVisible = false;

        HA_Left_MonthsPicker.IsVisible = false;
        HA_Left_MonthsLabel.IsVisible = false;

    }
}