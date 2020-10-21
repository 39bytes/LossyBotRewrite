using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class CompileModule : ModuleBase<SocketCommandContext>
    {
        private readonly CompileService _compileService;
        public CompileModule(CompileService service)
        {
            _compileService = service;
        }

        [Command("compile")]
        [Summary("Compiles code in the given language.")]
        public async Task CompileCommand([Remainder]string code)
        {
            if (!IsCode(code))
            {
                await ReplyAsync(Context.User.Mention + " Start and end your code with \"```\"!");
                return;
            }

            string lang = ParseLanguage(code);
            code = GetCodeToCompile(code);

            if (lang == "")
            {
                await ReplyAsync(Context.User.Mention + " Include what language you're using!");
            }

            OutputFormatter formatter = new OutputFormatter(lang, await _compileService.Compile(code, lang));
            await ReplyAsync(Context.User.Mention + " " + formatter.GetOutput());
        }

        private bool IsCode(string code)
        {
            if (code.Length < 6) return false;
            return code.StartsWith("```") && code.EndsWith("```");
        }

        private string ParseLanguage(string code)
        {
            string lang = code.Split("\n").FirstOrDefault();
            lang = lang.Trim('`');

            if (lang == "py")
                lang = "python3";
            if (lang == "cs")
                lang = "csharp";

            return lang;
        }

        string GetCodeToCompile(string code)
        {
            string lang = ParseLanguage(code);
            if (lang == "python3") lang = "py";
            if (lang == "csharp") lang = "cs";
            code = code.Replace("```" + lang, "");

            code = code.Remove(code.Length - 3);
            code = code.Replace("\n", "\\n");
            code = code.Replace('"'.ToString(), '\\'.ToString() + '"'.ToString());
            if (code.StartsWith("\\n")) code = code.Substring(2);
            return code;
        }

        private class OutputFormatter
        {
            string language, output, memory, cpuTime;

            public OutputFormatter(string language, string jsonIn)
            {
                JObject json = JObject.Parse(jsonIn);

                this.language = language;

                output = (string)json.SelectToken("output");
                memory = (string)json.SelectToken("memory");
                cpuTime = (string)json.SelectToken("cpuTime");
            }

            public string GetOutput()
            {
                if (output.Length > 1000) output = output.Substring(0, 1000) + ".....";
                return "```" + "\n" + output + "```Language: **" + language + "**  Memory: **" + memory + "**  Cpu Time: **" + cpuTime + "**";
            }
        }
    }
}
