﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;
using System.IO;

namespace Scumm.Engine.Resources.Scripts
{
    class ScriptLoader : ResourceLoader
    {
        ScriptManager scriptManager;
        SceneManager sceneManager;
        ScummStateV5 scummState;
        StreamWriter logFile;

        public ScriptLoader(ScriptManager scriptMngr, SceneManager sceneMngr, ScummStateV5 state, StreamWriter logFile)
        {
            scriptManager = scriptMngr;
            sceneManager = sceneMngr;
            scummState = state;
            this.logFile = logFile;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            uint blockSize;

            if (!parameters.ContainsKey("Type") && !parameters.ContainsKey("Position"))
            {
                blockSize = reader.FindDataBlock("SCRP");
            }
            else
            {
                if (parameters.ContainsKey("Type"))
                {
                    blockSize = reader.FindDataBlock((string)parameters["Type"]);
                    if ((string)parameters["Type"] == "LSCR")
                    {
                        byte id = reader.ReadByte();
                        resourceId = String.Format("LSCRP_{0}", id);
                        --blockSize;
                    }
                    else if ((string)parameters["Type"] == "EXCD")
                        resourceId = String.Format("SCRP_{0}", 10001);
                    else if ((string)parameters["Type"] == "ENCD")
                        resourceId = String.Format("SCRP_{0}", 10002);
                }
                else
                {
                    reader.BaseStream.Position = (long)parameters["Position"];
                    blockSize = (uint)parameters["Blocksize"];
                    blockSize += 8;
                }
            }
                
            // Read script Header information
            if (blockSize == 0)
            {
                throw new InvalidOperationException("Could not find the script header block.");
            }

            // Read the opcode blocks
            var data = reader.ReadBytes((int)blockSize - 8);
            var script = new ScriptV5(resourceId, data, scriptManager, resourceManager, sceneManager, scummState, this.logFile);
            return script;
        }
    }
}
