﻿using System;
using MiHomeLib.Events;
using Newtonsoft.Json.Linq;

namespace MiHomeLib.Devices
{
    public class MotionSensor : MiHomeDevice
    {
        public const string SensorKey = "motion";
        public override string Type => SensorKey;
        public event EventHandler OnMotion;
        public event EventHandler<NoMotionEventArgs> OnNoMotion;

        public MotionSensor(string sid) : base(sid) {}

        public float? Voltage { get; set; }

        public string Status { get; private set; }

        public int NoMotion { get; set; }

        public override void ParseData(string command)
        {
            var jObject = JObject.Parse(command);

            if (jObject["status"] != null)
            {
                Status = jObject["status"].ToString();

                if (Status == "motion")
                {
                    MotionDate = DateTime.Now;
                    OnMotion?.Invoke(this, EventArgs.Empty);
                }
            }

            if (jObject["no_motion"] != null)
            {
                NoMotion = int.Parse(jObject["no_motion"].ToString());
                
                OnNoMotion?.Invoke(this, new NoMotionEventArgs(NoMotion));
            }

            if (jObject["voltage"] != null && float.TryParse(jObject["voltage"].ToString(), out float v))
            {
                Voltage = v / 1000;
            }
        }

        public DateTime? MotionDate { get; private set; }

        public override string ToString()
        {
            return $"Status: {Status}, Voltage: {Voltage}V, NoMotion: {NoMotion}s";
        }

        
    }
}