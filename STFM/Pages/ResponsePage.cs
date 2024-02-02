namespace STFM.Pages;

public class ResponsePage : ContentPage
{

    STFM.Views.ResponseView CurrentResponseView;

    public ResponsePage(ref STFM.Views.ResponseView currentResponseView)
	{

		CurrentResponseView = currentResponseView;

		Content = CurrentResponseView;

  //      Content = new VerticalStackLayout
		//{
		//	Children = {
		//		new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "This is the response page!"
		//		}
		//	}
		//};
	}


    // Prevent closing: https://learn.microsoft.com/en-us/answers/questions/1336207/how-to-remove-close-and-maximize-button-for-a-maui


}