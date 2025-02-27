﻿using Cupscale.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cupscale.Main
{
    class GeneralOutputHandler
    {
        public static void HandleImpErrorMsgs(string log, Implementations.Implementation imp = null)
        {
            bool errored = false;

            if (imp == Implementations.Imps.esrganNcnn || imp == Implementations.Imps.realEsrganNcnn)
            {
                if (!errored && log.Contains("vkAllocateMemory"))
                {
                    Program.Cancel("NCNN ran out of memory.\nTry reducing the tile size and avoid " +
                        "running programs in the background (especially games) that take up your VRAM.");
                    errored = true;
                }

            }

            if (imp == Implementations.Imps.esrganPytorch)
            {
                if (log.ToLower().Contains("out of memory"))
                {
                    Program.ShowMessage("ESRGAN ran out of memory. Try reducing the tile size and avoid running programs in the background (especially games) that take up your VRAM.", "Error");
                    errored = true;
                }

                if (log.Contains("Python was not found"))
                {
                    Program.Cancel("Python was not found. Make sure you have a working Python 3 installation, or use the embedded runtime.");
                    errored = true;
                }

                if (log.Contains("ModuleNotFoundError"))
                {
                    Program.Cancel("You are missing Python dependencies. Make sure Pytorch and cv2 (opencv-python) are installed.");
                    errored = true;
                }

                if (log.Contains("RRDBNet"))
                {
                    Program.Cancel("Model appears to be incompatible!");
                    errored = true;
                }

                if (log.Contains("UnpicklingError"))
                {
                    Program.Cancel("Failed to load model!\nPossibly it's corrupted or in an unknown format.");
                    errored = true;
                }

                if (PreviewUi.currentMode == PreviewUi.MdlMode.Interp && (log.Contains("must match the size of tensor b") || log.Contains("KeyError: 'model.")))
                {
                    Program.Cancel("It seems like you tried to interpolate incompatible models!");
                    errored = true;
                }
            }

            if (!errored && log.Contains("failed"))
            {
                Program.Cancel($"Error occurred while running AI implementation: \n\n{log}\n\n");
                errored = true;
            }

            if (errored)
                Program.Cancel();
        }
    }
}
