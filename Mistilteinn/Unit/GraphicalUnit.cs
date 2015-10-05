using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mistilteinn.Helper;
using Mistilteinn.Texts;

namespace Mistilteinn.Unit
{
    public static class GraphicalUnit
    {
        public static async Task<ImageSource> ReadScript(Text text)
        {
            if (!Project.Current.IsPreviewEnable)
            {
                return null;
            }

            return await Task.Run(() =>
            {
                String bg = null;
                List<String> charas = null;

                if (text != null)
                {
                    bg = text.Background?.Path;
                    charas = text.Characters?.Select(c => c.Path).ToList();

                    DrawingVisual drawingVisual = new DrawingVisual();

                    using (var context = drawingVisual.RenderOpen())
                    {
                        if (!String.IsNullOrEmpty(bg))
                        {

                            var bgfile = FileHelper.FindFile(Project.Current.BackgroundPath, bg);

                            if (bgfile != null)
                            {
                                var bgImage = new BitmapImage(new Uri(bgfile));
                                context.DrawImage(bgImage, new Rect(0, 0, bgImage.PixelWidth, bgImage.PixelHeight));
                            }
                        }

                        charas = charas?.Distinct().ToList();

                        if ((charas?.Count ?? 0) > 0)
                        {
                            var charaImage = charas.Select(c =>
                            {
                                if (!String.IsNullOrEmpty(c))
                                {
                                    var charafile = FileHelper.FindFile(Project.Current.CharacterPath, c);

                                    if (charafile != null)
                                    {
                                        return new BitmapImage(new Uri(charafile));
                                    }
                                }

                                return null;
                            }).ToList();
                            charaImage.RemoveAll(b => b == null);

                            for (int index = 0; index < charaImage.Count; index++)
                            {
                                var image = charaImage[index];
                                switch (charaImage.Count)
                                {
                                    case 1:
                                        context.DrawImage(image, new Rect(0, 0, image.Width, image.Height));
                                        break;
                                    case 2:
                                        context.DrawImage(image, new Rect(-240 + (index * 440), 0, image.Width, image.Height));
                                        break;
                                    case 3:
                                        context.DrawImage(image, new Rect(-440 + (index * 440), 0, image.Width, image.Height));
                                        break;
                                }
                            }
                        }

                        context.Close();
                        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(1280, 720, 96, 96, PixelFormats.Pbgra32);
                        renderBitmap.Render(drawingVisual);
                        if (renderBitmap.CanFreeze)
                        {
                            renderBitmap.Freeze();
                        }
                        return renderBitmap;
                    }
                }
                return null;
            });

        }
    }
}
