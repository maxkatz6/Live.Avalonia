using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace Live.Avalonia.Sample
{
    public class App : Application, ILiveView
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            var window = new LiveViewHost(this, Console.WriteLine);
            window.StartWatchingSourceFilesForHotReloading();
            window.Show();
            
            base.OnFrameworkInitializationCompleted();
        }

        public object CreateView(Window window) => new Grid
        {
            Children =
            {
                new Border
                {
                    Width = 500,
                    Height = 150,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Classes = Classes.Parse("Card"),
                    Child = new StackPanel
                    {
                        Children = 
                        { 
                            new TextBlock
                            {
                                Margin = new Thickness(10, 10, 10, 10),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Text = "This is Avalonia hot reloading content!"
                            },
                            new Button
                            {
                                Content = "I'm a button!"
                            }
                        }
                    }
                }
            }
        };
    }
}
