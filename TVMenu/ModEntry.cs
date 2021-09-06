using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace YourProjectName
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        enum MenuItem { WEATHER, FORTUNE, RECIPE, TIPS, FISHINGTIP};

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }


        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            List<MenuItem> todaysItems = this.getTodaysItems();
            List<string> todaysReport = this.genTodaysReport(todaysItems);
            

            log("Day Started hehe");
            foreach(string s in todaysReport)
            {
                log(s);
            }
        

        }

        private List<string> genTodaysReport(List<MenuItem> items)
        {
            List<string> report = new List<string>();
            TV t = new TV();
            foreach (MenuItem i in items)
            {
                switch (i)
                {
                    case MenuItem.WEATHER:
                        report.Add("Weather Forecast:");
                        string weather = this.Helper.Reflection.GetMethod(t, "getWeatherForecast").Invoke<string>();
                        report.Add(weather);
                        break;
                    case MenuItem.FORTUNE:
                        report.Add("Today's Luck: ");
                        string luck = this.Helper.Reflection.GetMethod(t, "getFortuneForecast").Invoke<string>(Game1.player);
                        report.Add(luck);
                        break;
                    case MenuItem.TIPS:
                        report.Add("Livin' Off The Land");
                        string tips = this.Helper.Reflection.GetMethod(t, "getTodaysTip").Invoke<string>();
                        report.Add(tips);
                        break;
                    case MenuItem.RECIPE:
                        string title = "The Queen of Sauce";
                        string[] recipe = this.Helper.Reflection.GetMethod(t, "getWeeklyRecipe").Invoke<string[]>();
                        if(recipe[1].Contains("You already know how to cook"))
                        {
                            if(recipe[1].ToLower().Contains("carp surprise"))
                            {
                                title += ": Rerun for Carp Surprise";
                            }
                            else if (recipe[1].ToLower().Contains("melon"))
                            {
                                title += ": Rerun for Melon";
                            }
                            else
                            {
                                string name = recipe[0].Split('!')[0];
                                title += $": Rerun for {name}";
                            }
                            report.Add(title);
                        }
                        else
                        {
                            report.Add(title);
                            report.Add(recipe[0]);
                        }
                        break;
                }
            }

            return report;
        }

        private List<MenuItem> getTodaysItems()
        {
            List<MenuItem> todaysItems = new List<MenuItem>();
            todaysItems.Add(MenuItem.WEATHER);
            todaysItems.Add(MenuItem.FORTUNE);
            string day = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (day.Equals("Mon") || day.Equals("Thu"))
            {
                todaysItems.Add(MenuItem.TIPS);
            }
            if (day.Equals("Sun") || (day.Equals("Wed") && Game1.stats.DaysPlayed > 7))
            {
                todaysItems.Add(MenuItem.RECIPE);
            }
            if (Game1.player.mailReceived.Contains("pamNewChannel"))
            {
                todaysItems.Add(MenuItem.FISHINGTIP);
            }

            return todaysItems;
        }

        private void log(String l)
        {
            this.Monitor.Log(l, LogLevel.Debug);
        }
    }
}