﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace VTOLVR_ModLoader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static string[] pilotsCFG;
        //Moving Window
        private bool holdingDown;
        private Point lm = new Point();

        public Settings()
        {
            InitializeComponent();
            devConsoleCheckbox.IsChecked = MainWindow.devConsole;                    
            FindPilots();
            AddDefaultScenarios();

            if (MainWindow.pilotSelected != null)
            {
                foreach (Pilot p in PilotDropdown.ItemsSource)
                {
                    if (p.Name == MainWindow.pilotSelected.Name)
                    {
                        PilotDropdown.SelectedItem = p;
                        break;
                    }
                }
            }

            if (MainWindow.scenarioSelected != null)
            {
                foreach (Scenario s in ScenarioDropdown.ItemsSource)
                {
                    if (s.ID == MainWindow.scenarioSelected.ID)
                    {
                        ScenarioDropdown.SelectedItem = s;
                        break;
                    }
                }
            }
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            Quit();
        }
        private void Quit()
        {
            Settings s = MainWindow.settings;
            MainWindow.settings = null;
            s.Close();
        }
        #region Moving Window
        private void TopBarDown(object sender, MouseButtonEventArgs e)
        {
            holdingDown = true;
            lm = Mouse.GetPosition(this);
        }

        private void TopBarUp(object sender, MouseButtonEventArgs e)
        {
            holdingDown = false;
        }

        private void TopBarMove(object sender, MouseEventArgs e)
        {
            if (holdingDown)
            {
                this.Left += Mouse.GetPosition(this).X - lm.X;
                this.Top += Mouse.GetPosition(this).Y - lm.Y;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {

        }

        private void TopBarLeave(object sender, MouseEventArgs e)
        {
            holdingDown = false;
        }

        #endregion

        private void DevConsole(object sender, RoutedEventArgs e)
        {
            if (devConsoleCheckbox.IsChecked == true)
                MainWindow.devConsole = true;
            else if (devConsoleCheckbox.IsChecked == false)
                MainWindow.devConsole = false;
        }

        private void CreateInfo(object sender, RoutedEventArgs e)
        {
            Mod newMod = new Mod(modName.Text, modDescription.Text);

            Directory.CreateDirectory(MainWindow.root + $"\\mods\\{modName.Text}");

            using (FileStream stream = new FileStream(MainWindow.root + $"\\mods\\{modName.Text}\\info.xml", FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(Mod));
                xml.Serialize(stream, newMod);
            }

            MessageBox.Show("Created info.xml in \n\"" + MainWindow.root + $"\\mods\\{modName.Text}\"", "Created Info.xml", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FindPilots()
        {
            if (pilotsCFG == null)
                pilotsCFG = File.ReadAllLines(MainWindow.vtolFolder + @"\SaveData\pilots.cfg");
            string result;
            List<Pilot> pilots = new List<Pilot>(1) { new Pilot("No Selection")};
            for (int i = 0; i < pilotsCFG.Length; i++)
            {
                result = Helper.ClearSpaces(pilotsCFG[i]);
                if (result.Contains("pilotName="))
                {
                    pilots.Add(new Pilot(result.Replace("pilotName=",string.Empty)));
                }
            }

            if (pilots.Count > 0)
            {
                PilotDropdown.ItemsSource = pilots;
                PilotDropdown.SelectedIndex = 0;
            }
        }

        private void AddDefaultScenarios()
        {
            ScenarioDropdown.ItemsSource = new Scenario[]
            {
                new Scenario("No Selection","",""),
                new Scenario("AV-42C - Preparations", "av42cTheIsland", "01_preparations"),
                new Scenario("AV-42C - Minesweeper", "av42cTheIsland", "02_minesweeper"),
                new Scenario("AV-42C - Redirection", "av42cTheIsland", "03_redirection"),
                new Scenario("AV-42C - Open Water", "av42cTheIsland", "04_openWater"),
                new Scenario("AV-42C - Silent Island", "av42cTheIsland", "05_silentIsland"),
                new Scenario("AV-42C - Darkness", "av42cTheIsland", "06_darkness"),
                new Scenario("AV-42C - Island Defense", "av42cTheIsland", "07_islandDefense"),
                new Scenario("AV-42C - Free Flight", "av42cQuickFlight", "freeFlight"),
                new Scenario("AV-42C - Target Practice", "av42cQuickFlight", "targetPractice"),
                new Scenario("AV-42C - Aerial Refueling Practice", "av42cQuickFlight", "aerialRefuelPractice"),
                new Scenario("AV-42C - Naval Landing Practice", "av42cQuickFlight", "carrierLanding"),
                new Scenario("F/A-26B - Free Flight", "fa26bFreeFlight", "Free Flight"),
                new Scenario("F/A-26B - Target Practice", "fa26bFreeFlight", "targetPractice"),
                new Scenario("F/A-26B - Carrier Landing Practice", "fa26bFreeFlight", "carrierLandingPractice"),
                new Scenario("F/A-26B - FA-26 Aerial Refuel Practice", "fa26bFreeFlight", "fa26Refuel"),
                new Scenario("F/A-26B - 2v2 Air Combat", "fa26bFreeFlight", "2v2dogfight"),
                new Scenario("F/A-26B - Difficult Mission", "fa26bFreeFlight", "FA26Difficult"),
                new Scenario("F/A-26B - July 4th", "j4Campaign", "j4"),
                new Scenario("F/A-26B - Base Defense", "fa26-opDesertCobra", "mission1"),
                new Scenario("F/A-26B - Retalliation", "fa26-opDesertCobra", "mission2"),
                new Scenario("F/A-26B - Strike on Naval Test Lake", "fa26-opDesertCobra", "mission3"),
                new Scenario("F/A-26B - Tanker Escort", "fa26-opDesertCobra", "mission4"),
                new Scenario("F/A-26B - Departure", "fa26-opDesertCobra", "mission5"),
                new Scenario("F/A-26B - Northern Assault", "fa26-opDesertCobra", "mission6"),
                new Scenario("F/A-26B - Striking Oil", "fa26-opDesertCobra", "mission7"),
                new Scenario("F-45A - Free Flight", "f45-quickFlight", "f45-freeFlight"),
                new Scenario("F-45A - Stealth Strike", "f45-quickFlight", "f45_quickMission1")
            };
        }
        private void PilotChanged(object sender, EventArgs e)
        {
            MainWindow.pilotSelected = (Pilot)PilotDropdown.SelectedItem;
        }

        private void ScenarioChanged(object sender, EventArgs e)
        {
            MainWindow.scenarioSelected = (Scenario)ScenarioDropdown.SelectedItem;
        }
    }

    public class Pilot
    {
        public string Name { get; set; }
        public Pilot(string name)
        {
            Name = name;
        }
    }

    public class Scenario
    {
        public string Name { get; set; }
        public string ID;
        public string cID;
        public Scenario(string name, string cID,string iD)
        {
            Name = name;
            ID = iD;
            this.cID = cID;
        }
    }

}

