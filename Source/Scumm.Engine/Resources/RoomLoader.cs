﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;
using Scumm.Engine.Resources.Scripts;

namespace Scumm.Engine.Resources.Loaders
{
    public class RoomLoader : ResourceLoader
    {
        public RoomLoader()
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            var room = new Room(resourceId);
            var roomOffset = (uint)reader.BaseStream.Position;

            // Read Room Header information
            if (reader.FindDataBlock("RMHD", roomOffset) == 0)
            {
                throw new InvalidOperationException("Could not find the room header block.");
            }

            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var objectsCount = reader.ReadUInt16();

            // Read palette data
            if (reader.FindDataBlock("CLUT", roomOffset) == 0)
            {
                throw new InvalidOperationException("Could not find the room palette block.");
            }

            // Load only the first palette for now
            room.Palette = new Color[256];
            for (int i = 0; i < 256; ++i)
            {
                room.Palette[i].R = reader.ReadByte();
                room.Palette[i].G = reader.ReadByte();
                room.Palette[i].B = reader.ReadByte();
            }

            room.Objects = new Object[objectsCount];
            for (int i = 0; i < objectsCount; ++i)
            {
                Object obj = ResourceManager.Load<Object>("OBJC", new Dictionary<string, object>());
                room.Objects[i] = obj;
            }

            // Read entry/exit scripts
            room.ExitScript = ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "EXCD" } });
            room.EntryScript = ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "ENCD" } });

            // Read background image
            room.BackgroundImage = ResourceManager.Load<Image>("RMIM", new Dictionary<string, object>() { { "Width", (int)width }, { "Height", (int)height }, { "RoomOffset", roomOffset }, { "RoomPalette", room.Palette } });

            return room;
        }
    }
}
