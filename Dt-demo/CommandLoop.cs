using Azure;
using Azure.DigitalTwins.Core;
using Microsoft.Azure.DigitalTwins.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SampleClientApp
{
    public class CommandLoop
    {
        private readonly DigitalTwinsClient client;

        public CommandLoop(DigitalTwinsClient client)
        {
            this.client = client;
            CliInitialize();
        }
        /// <summary>
        /// Get a twin with the specified id in a cycle
        /// </summary>
        /// <param name='twin_id0'>
        /// Id of an existing twin
        /// </param>
        public Task CommandObserveProperties(string[] cmd)
        {
            if (cmd.Length < 3)
            {
                Log.Error("Please provide at least one pair of twin-id and property name to observe");
                return Task.CompletedTask;
            }
            string[] args = cmd.Skip(1).ToArray();
            if (args.Length % 2 != 0 || args.Length > 8)
            {
                Log.Error("Please provide pairs of twin-id and property names (up to 4) to observe");
                return Task.CompletedTask;
            }
            Log.Alert($"Starting observation...");
            var state = new TimerState
            {
                Arguments = args
            };
            var stateTimer = new Timer(CheckState, state, 0, 2000);
            Log.Alert("Press any key to end observation");
            Console.ReadKey(true);
            state.IsActive = false;
            stateTimer.Dispose();

            return Task.CompletedTask;
        }

        private class TimerState
        {
            public bool IsActive { get; set; } = true;
            public string[] Arguments { get; set; }
        }

        private void CheckState(object state)
        {
            TimerState ts = state as TimerState;
            if (ts == null || ts.IsActive == false)
                return;

            for (int i = 0; i < ts.Arguments.Length; i += 2)
            {
                try
                {   //Temporary fix while time format is being updated on the backend.
                    //Revert object back to BasicDigitalTwin after December 19th 2020
                    Response<object> res0 = client.GetDigitalTwin<object>(ts.Arguments[i]);
                    if (res0 != null)
                        LogProperty(JsonSerializer.Serialize(res0.Value), ts.Arguments[i + 1]);
                }
                catch (RequestFailedException e)
                {
                    Log.Error($"Error {e.Status}: {e.Message}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error: {ex}");
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Helper Functions
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Cast a string to its intended type
        public object ConvertStringToType(string schema, string val)
        {
            switch (schema)
            {
                case "boolean":
                    return bool.Parse(val);
                case "double":
                    return double.Parse(val);
                case "float":
                    return float.Parse(val);
                case "integer":
                case "int":
                    return int.Parse(val);
                case "datetime":
                    return DateTime.Parse(val);
                case "duration":
                    return int.Parse(val);
                case "string":
                default:
                    return val;
            }
        }

        // Log a JSON serialized object to the command prompt
        public void LogResponse(string res, string type = "")
        {
            if (type != "")
                Log.Alert($"{type}: \n");
            else
                Log.Alert("Response:");

            if (res == null)
                Log.Out("Null response");
            else
                Console.WriteLine(PrettifyJson(res));
        }

        // Log temperature changes in sample app
        public void LogProperty(string res, string propName = "Temperature")
        {
            var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(res);

            if (!obj.TryGetValue("$dtId", out object dtid))
                dtid = "<$dtId not found>";

            if (!obj.TryGetValue(propName, out object value))
                value = "<property not found>";

            Console.WriteLine($"$dtId: {dtid}, {propName}: {value}");
        }

        private string PrettifyJson(string json)
        {
            object jsonObj = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Console UI
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private struct CliInfo
        {
            public string Help { get; set; }
            public Func<string[], Task> Command { get; set; }
            public CliCategory Category { get; set; }
        }

        private enum CliCategory
        {
            ADTModels,
            ADTTwins,
            ADTQuery,
            ADTRoutes,
            SampleScenario,
            SampleTools,
        }

        private Dictionary<string, CliInfo> commands;
        private void CliInitialize()
        {
            commands = new Dictionary<string, CliInfo>
            {
                { "Help", new CliInfo { Command=CommandHelp, Category = CliCategory.SampleTools, Help="List all commands" } },
                { "ObserveProperties", new CliInfo { Command=CommandObserveProperties, Category = CliCategory.SampleScenario, Help="<twin id> <propertyName> <twin-id> <property name>... observes the selected properties on the selected twins" } },
                { "Exit", new CliInfo { Command=CommandExit, Category = CliCategory.SampleTools, Help="Exits the program" } },
            };
        }

        public Task CommandExit(string[] args = null)
        {
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        public Task CommandHelp(string[] args = null)
        {
            Log.Ok("This demo app observes the properties of the Digital Twin.");
            Log.Ok("Type <'Observeproperties'>, followed by <'dtid'> to start observation.");
            Log.Ok("");
            Log.Ok("press an key to return from observing");
            Log.Out("");
            if (args != null && args.Length < 2)
            {
                Log.Alert("Scenario Demo Commands:");
                CliPrintCategoryCommands(CliCategory.SampleScenario);
                Log.Alert("Some ADT Commands for Model Management:");
                CliPrintCategoryCommands(CliCategory.ADTModels);
                Log.Alert("Some ADT Commands for Twins:");
                CliPrintCategoryCommands(CliCategory.ADTTwins);
                Log.Alert("ADT Commands for Query:");
                CliPrintCategoryCommands(CliCategory.ADTQuery);
                Log.Alert("ADT Commands for Event Routes:");
                CliPrintCategoryCommands(CliCategory.ADTRoutes);
                Log.Alert("Others:");
                CliPrintCategoryCommands(CliCategory.SampleTools);
            }

            return Task.CompletedTask;
        }

        private void CliPrintCategoryCommands(CliCategory cat)
        {
            var clist = commands.Where(p => p.Value.Category == cat).Select(p => new { Cmd = p.Key, Help = p.Value.Help });
            foreach (var item in clist)
            {
                Log.Out($"  {item.Cmd} {item.Help}");
            }
        }

        public async Task CliCommandInterpreter()
        {
            Log.Out("");
            await CommandHelp(new string[] { "help", "headerOnly" });
            Log.Out("");
            while (true)
            {
                try
                {
                    string command = Console.ReadLine().Trim();
                    string[] commandArr = SplitArgs(command);
                    string verb = commandArr[0].ToLower();
                    if (!string.IsNullOrEmpty(verb))
                    {
                        var cmd = commands
                            .Where(p => p.Key.ToLower() == verb)
                            .Select(p => p.Value.Command)
                            .Single();
                        await cmd(commandArr);
                    }
                }
                catch (Exception)
                {
                    Log.Error("Invalid command. Please type 'help' for more information.");
                }
            }
        }

        private string[] SplitArgs(string arg)
        {
            int quotecount = arg.Count(x => x == '"');
            if (quotecount % 2 != 0)
            {
                Log.Alert("Your command contains an uneven number of quotes. Was that intended?");
            }
            string[] segments = arg.Split('"', StringSplitOptions.RemoveEmptyEntries);
            var elements = new List<string>();
            for (int i = 0; i < segments.Length; i++)
            {
                if (i % 2 == 0)
                {
                    string[] parts = segments[i].Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ps in parts)
                        elements.Add(ps.Trim());
                }
                else
                {
                    elements.Add(segments[i].Trim());
                }
            }
            return elements.ToArray();
        }
    }
}