using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using static RestIOS.Clases.ValidaRostro;
using static System.Net.Mime.MediaTypeNames;

class FacialRecognition
{
    private CascadeClassifier _faceCascade;
    private CascadeClassifier _eyeCascade;

    public FacialRecognition()
    {
        // Cargar el clasificador en cascada para el rostro
        _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
        //_eyeCascade = new CascadeClassifier("haarcascade_eye.xml");
        //_faceCascade = new CascadeClassifier("haarcascade_frontalcatface.xml");
        //_faceCascade = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml"); 
    }

    // Detectar rostro en una imagen
    public Rectangle DetectFace(Mat image)
    {
        // Convertir la imagen a escala de grises
        UMat grayImage = new UMat();

        CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

        // Detectar rostros en la imagen
        var faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 5, Size.Empty);

        if (faces.Length > 0)
        {
            return faces[0]; // Devolver el primer rostro detectado
        }

        return Rectangle.Empty;
    }

    // Extraer la región del rostro
    public Mat ExtractFace(Mat image, Rectangle faceRegion)
    {
        return new Mat(image, faceRegion);
    }

    // Función para convertir Base64 a Mat
    /*public Mat ConvertBase64ToMat(string base64String)
    {
        // Convertir la cadena Base64 a un arreglo de bytes
        byte[] imageBytes = Convert.FromBase64String(base64String);

        // Crear un MemoryStream a partir de los bytes de la imagen
        using (MemoryStream ms = new MemoryStream(imageBytes))
        {
            // Leer la imagen desde el MemoryStream y convertirla en un objeto Mat usando Mat constructor
            Mat image = new Mat(ms);

            return image;
        }
    }*/

    public Mat ConvertBase64ToMat(string rutaArchivo)
    {
        Mat image = new Mat(rutaArchivo);

        return image;
    }

    public String ConvertBase64ToArchivo(string base64String)
    {
        string uniqueName = "";

        try
        {
            // Convertir la cadena Base64 a un arreglo de bytes
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Escribir los bytes en un archivo de imagen
            uniqueName = "imgs/Temp/" + "image_" + Guid.NewGuid().ToString() + ".jpg";

            File.WriteAllBytes(uniqueName, imageBytes);

            Console.WriteLine("Imagen guardada en: " + uniqueName);
        }
        catch (Exception ex)
        {
            uniqueName = "Error";
        }

        return uniqueName;
    }

    // Comparar dos rostros (simplemente comparando la similitud de las imágenes)
    public bool CompareFaces(Mat face1, Mat face2)
    {
        // Para este ejemplo, utilizamos un enfoque sencillo: comparar el histograma de la imagen.
        UMat hist1 = new UMat();
        UMat hist2 = new UMat();

        // Calculando el histograma para cada canal BGR por separado
        /*CvInvoke.CalcHist(
            new UMat[] { face1 }, // Imagen a procesar
            new int[] { 0 }, // Canal 0 (azul)
            null, // Máscara (null si no se usa)
            hist1, // Histograma calculado
            new int[] { 256 }, // Número de bins
            new RangeF[] { new RangeF(0, 256) } // Rango de los valores (0-255)
        );*/

        /*CvInvoke.CalcHist(
        new UMat[] { face1 },
        new int[] { 0, 1, 2 }, // BGR
        null,
        hist1,
        new int[] { 256, 256, 256 }, // 256 bins para cada canal
        new RangeF[] { new RangeF(0, 256), new RangeF(0, 256), new RangeF(0, 256) }
        );*/

        VectorOfMat vou = new VectorOfMat();

        vou.Push(face1);

        // Calcular los histogramas
        //CvInvoke.CalcHist(new UMat[] { face1 }, new int[] { 0 }, null, hist1, new int[] { 256 }, new float[] { 0, 256 }, false);
        //CvInvoke.CalcHist(new UMat[] { face2 }, new int[] { 0 }, null, hist2, new int[] { 256 }, new RangeF[] { new RangeF(0, 256) });

        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist1, new int[] { 256 }, new float[] { 0, 256 }, false);

        vou = new VectorOfMat();
        vou.Push(face2);

        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist2, new int[] { 256 }, new float[] { 0, 256 }, false);

        // Normalizar los histogramas
        CvInvoke.Normalize(hist1, hist1, 0, 255, NormType.MinMax, DepthType.Cv32F);
        CvInvoke.Normalize(hist2, hist2, 0, 255, NormType.MinMax, DepthType.Cv32F);

        // Comparar los histogramas
        double similarity = CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Correl); //HistCompMethod 

        // Si la similitud es alta, asumimos que las caras son similares
        return similarity > 0.80; // Este valor puede ajustarse según el umbral deseado
    }

    public Crostro ValidaRostro1(Mat imagenParametro, Mat imagenGuardada, string archivo)
    {
        Crostro cRostro = new Crostro();

        // Convertir la imagen a escala de grises
        UMat grayImage = new UMat();

        CvInvoke.CvtColor(imagenParametro, grayImage, ColorConversion.Bgr2Gray);

        // Detectar rostros en la imagen
        var faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 3);

        if (archivo == "986774")
        {
            string x1 = "";
        }

        if (faces.Length > 0)
        {
            //Obtener valores del rectangulo
            int x = faces[0].X;
            int y = faces[0].Y;
            int width = faces[0].Width;
            int height = faces[0].Height;

            cRostro.status = "1";
            cRostro.resultado = "Ninguna cara coincide.";

            try
            {
                Mat faceImage1 = ExtractFace(imagenParametro, faces[0]);

                // Convertir la imagen a escala de grises
                CvInvoke.CvtColor(imagenGuardada, grayImage, ColorConversion.Bgr2Gray);

                // Detectar rostros en la imagen
                faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 3);

                if (faces.Length > 0)
                {
                    // Modificar las propiedades de cada rectángulo
                    Rectangle face = faces[0];

                    // Asignar nuevos valores a X, Y, Width y Height
                    face.X = x;           // Nuevo valor para la coordenada X
                    face.Y = y;           // Nuevo valor para la coordenada Y
                    face.Width = width;   // Nuevo valor para el ancho
                    face.Height = height; // Nuevo valor para la altura

                    // Reasignar el rectángulo modificado de nuevo a la lista de caras
                    faces[0] = face;

                    try
                    {
                        Mat faceImage2 = ExtractFace(imagenGuardada, faces[0]);

                        // Comparar las caras
                        bool facesAreSimilar = CompareFaces(faceImage1, faceImage2);

                        if (facesAreSimilar)
                        {
                            // Mostrar el resultado
                            CvInvoke.Imshow("Imagen", imagenParametro);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            CvInvoke.Imshow("Imagen", imagenGuardada);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            cRostro.status = "0";
                            cRostro.resultado = "La cara coincide con la del archivo: " + archivo;
                        }
                        else
                        {
                            /*
                            // Dibujar un rectángulo alrededor del rostro
                            CvInvoke.Rectangle(imagenParametro, faces[0], new MCvScalar(0, 0, 255)); //Rojo

                            // Extraer la región de interés (ROI) del rostro en la imagen en escala de grises (en UMat)
                            UMat roiGris = new UMat(grayImage, faces[0]);  // ROI en UMat

                            // Detectar ojos dentro del rostro
                            var ojos = _eyeCascade.DetectMultiScale(roiGris);

                            foreach (var ojo in ojos)
                            {
                                // Dibujar un rectángulo alrededor de los ojos detectados
                                var eyeRect = new Rectangle(faces[0].X + ojo.X, faces[0].Y + ojo.Y, ojo.Width, ojo.Height);

                                CvInvoke.Rectangle(imagenParametro, eyeRect, new MCvScalar(0, 255, 0)); //Verde
                            }

                            // Comparar las ojos
                            facesAreSimilar = CompareFaces(imagenParametro, imagenGuardada);

                            if (facesAreSimilar)
                            {
                                cRostro.status = "0";
                                cRostro.resultado = "La cara coincide con la del archivo: " + archivo;

                            }
                            else
                            {
                                cRostro.status = "1";
                                cRostro.resultado = "Ninguna cara coincide.";
                            }*/
                        }

                        /*
                        // Dibujar un rectángulo alrededor del rostro
                        CvInvoke.Rectangle(imagenParametro, face, new MCvScalar(0, 0, 255)); //Rojo

                        // Extraer la región de interés (ROI) del rostro en la imagen en escala de grises (en UMat)
                        UMat roiGris = new UMat(grayImage, face);  // ROI en UMat

                        // Detectar ojos dentro del rostro
                        var ojos = _eyeCascade.DetectMultiScale(roiGris);

                        foreach (var ojo in ojos)
                        {
                            // Dibujar un rectángulo alrededor de los ojos detectados
                            var eyeRect = new Rectangle(face.X + ojo.X, face.Y + ojo.Y, ojo.Width, ojo.Height);

                            CvInvoke.Rectangle(imagenParametro, eyeRect, new MCvScalar(0, 255, 0)); //Verde
                        }

                        // Mostrar el resultado
                        CvInvoke.Imshow("Imagen", imagenParametro);
                        CvInvoke.WaitKey(0);
                        CvInvoke.DestroyAllWindows();
                        CvInvoke.Imshow("Imagen", imagenGuardada);
                        CvInvoke.WaitKey(0);
                        CvInvoke.DestroyAllWindows();*/
                    }
                    catch
                    {
                        cRostro.status = "1";
                        cRostro.resultado = "Error al comparar rectangulo.";
                    }
                }
                else
                {
                    cRostro.status = "1";
                    cRostro.resultado = "No se detectaron rostros en la imágen guardada.";
                }
            }
            catch (Exception ex)
            {
                cRostro.status = "1";
                cRostro.resultado = "Error al comparar rectangulo.";
            }
        }
        else
        {
            cRostro.status = "1";
            cRostro.resultado = "No se detectaron rostros en la imágen parámetro.";
        }

        return cRostro;
    }

    public Crostro ValidaRostro2(Mat imagenParametro, Mat imagenGuardada, string archivo)
    {
        Crostro cRostro = new Crostro();

        cRostro.status = "1";
        cRostro.resultado = "Ninguna cara coincide.";

        // Cargar la imagen
        //string imagePath = @"ruta\a\tu\imagen.jpg";
        var image = new Image<Bgr, byte>(imagenParametro);

        // Convertir la imagen a escala de grises
        UMat grayImage = new UMat();

        CvInvoke.CvtColor(imagenParametro, grayImage, ColorConversion.Bgr2Gray);
        //grayImage.Save(@"rostro_extraidoGray.jpg");

        //var grayImage1 = image.Convert<Gray, byte>();
        //image.Save(@"rostro_extraidoGray.jpg");

        // Detectar los rostros en la imagen
        var faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);

        if (faces.Length > 0)
        {
            image.Draw(faces[0], new Bgr(Color.Green), 3);

            // Extraer la región del rostro
            var faceRegion = image.GetSubRect(faces[0]);
            faceRegion.Save(@"rostro_extraido.jpg");

            image = new Image<Bgr, byte>(imagenGuardada);
            CvInvoke.CvtColor(imagenGuardada, grayImage, ColorConversion.Bgr2Gray);
            grayImage.Save(@"rostro_extraidoGray1.jpg");
            faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);

            if (faces.Length > 0)
            {
                image.Draw(faces[0], new Bgr(Color.Green), 3);

                // Extraer la región del rostro
                var faceRegion1 = image.GetSubRect(faces[0]);
                faceRegion1.Save(@"rostro_extraido1.jpg");

                // Convertir las imágenes Base64 a objetos Mat
                Mat image1 = ConvertBase64ToMat(@"rostro_extraido.jpg");
                Mat image2 = ConvertBase64ToMat(@"rostro_extraido1.jpg");

                // Comparar las caras
                bool facesAreSimilar = CompareFaces(image1, image2);

                if (facesAreSimilar)
                {
                    // Mostrar el resultado
                    CvInvoke.Imshow("Imagen", image1);
                    CvInvoke.WaitKey(0);
                    CvInvoke.DestroyAllWindows();

                    CvInvoke.Imshow("Imagen", image2);
                    CvInvoke.WaitKey(0);
                    CvInvoke.DestroyAllWindows();

                    cRostro.status = "0";
                    cRostro.resultado = "La cara coincide con la del archivo: " + archivo;
                }
            }
            else
            {
                cRostro.status = "1";
                cRostro.resultado = "No se detectaron rostros en la imágen guardada.";
            }
        }
        else {
            cRostro.status = "1";
            cRostro.resultado = "No se detectaron rostros en la imágen parámetro.";            
        }
        
        return cRostro;
    }

    public Crostro ValidaRostro(Mat imagenParametro)
    {
        Crostro cRostro = new Crostro();

        try{
            cRostro.status = "1";
            cRostro.resultado = "Ninguna cara coincide.";

            // Cargar la imagen
            var image = new Image<Bgr, byte>(imagenParametro);

            // Convertir la imagen a escala de grises
            UMat grayImage = new UMat();

            CvInvoke.CvtColor(imagenParametro, grayImage, ColorConversion.Bgr2Gray);
            grayImage.Save(@"rostro_extraidoGray.jpg");

            // Detectar los rostros en la imagen
            var faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);

            if (faces.Length > 0)
            {
                image.Draw(faces[0], new Bgr(Color.Green), 3);

                // Extraer la región del rostro
                var faceRegion = image.GetSubRect(faces[0]);
                faceRegion.Save(@"rostro_extraido.jpg");

                string ruta = Directory.GetCurrentDirectory() + "\\imgs";

                // Obtener todos los archivos en el directorio especificado
                string[] archivos = Directory.GetFiles(ruta);

                // Mostrar los archivos encontrados
                foreach (var archivo in archivos)
                {
                    string rutaImage2 = archivo; // "imgs/986774.jpg";

                    // Convertir las imágenes Base64 a objetos Mat
                    Mat imagenGuardada = ConvertBase64ToMat(rutaImage2);

                    image = new Image<Bgr, byte>(imagenGuardada);

                    CvInvoke.CvtColor(imagenGuardada, grayImage, ColorConversion.Bgr2Gray);
                    grayImage.Save(@"rostro_extraidoGray1.jpg");

                    faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);

                    if (faces.Length > 0)
                    {
                        image.Draw(faces[0], new Bgr(Color.Green), 3);

                        // Extraer la región del rostro
                        var faceRegion1 = image.GetSubRect(faces[0]);
                        faceRegion1.Save(@"rostro_extraido1.jpg");

                        // Convertir las imágenes Base64 a objetos Mat
                        Mat image1 = ConvertBase64ToMat(@"rostro_extraido.jpg");
                        Mat image2 = ConvertBase64ToMat(@"rostro_extraido1.jpg");

                        // Comparar las caras
                        bool facesAreSimilar = CompareFaces(image1, image2);

                        if (facesAreSimilar)
                        {
                            // Mostrar el resultado
                            CvInvoke.Imshow("Imagen", image1);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            CvInvoke.Imshow("Imagen", image2);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            cRostro.status = "0";
                            cRostro.archivo = Path.GetFileName(archivo).Substring(0, Path.GetFileName(archivo).LastIndexOf("."));
                            cRostro.resultado = "La cara coincide con la del archivo: " + cRostro.archivo;
                        }
                    }
                    else
                    {
                        cRostro.status = "1";
                        cRostro.resultado = "No se detectaron rostros en la imágen guardada.";
                    }
                }
            }
            else
            {
                cRostro.status = "1";
                cRostro.resultado = "No se detectaron rostros en la imágen parámetro.";
            }
        } 
        catch(Exception ex) {
            cRostro.status = "1";
            cRostro.resultado = ex.Message;
        }

        return cRostro;

    }

    public Crostro ValidaRostroLentes(Mat imagenParametro)
    {
        Crostro cRostro = new Crostro();

        try
        {
            cRostro.status = "1";
            cRostro.resultado = "Ninguna cara coincide.";

            // Cargar la imagen
            var image1 = new Image<Bgr, byte>(imagenParametro);

            // Convertir la imagen a escala de grises
            var grayImage1 = image1.Convert<Gray, byte>();

            // Detectar los rostros en la imagen
            var faces1 = _faceCascade.DetectMultiScale(grayImage1, 1.1, 10, Size.Empty);

            if (faces1.Length > 0)
            {
                string ruta = Directory.GetCurrentDirectory() + "\\imgs";

                // Obtener todos los archivos en el directorio especificado
                string[] archivos = Directory.GetFiles(ruta);

                // Mostrar los archivos encontrados
                foreach (var archivo in archivos)
                {
                    string rutaImage2 = archivo; // "imgs/986774.jpg";

                    // Convertir la imágen guardada Base64 a objetos Mat
                    Mat imagenGuardada = ConvertBase64ToMat(rutaImage2);

                    // Convertir la imagen a escala de grises
                    var image2 = new Image<Bgr, byte>(imagenGuardada);

                    // Convertir las imáge a escala de grises
                    var grayImage2 = image2.Convert<Gray, byte>();

                    // Detectar los rostros en la imagen
                    var faces2 = _faceCascade.DetectMultiScale(grayImage2, 1.1, 10, Size.Empty);

                    if (faces2.Length > 0)
                    {
                        // Extraer los rostros
                        var face1 = image1.GetSubRect(faces1[0]);
                        var face2 = image2.GetSubRect(faces2[0]);

                        // Convertir a escala de grises para la comparación
                        var grayFace1 = face1.Convert<Gray, byte>();
                        var grayFace2 = face2.Convert<Gray, byte>();

                        /*
                        // Redimensionar los rostros a un tamaño común
                        grayFace1 = grayFace1.Resize(100, 100, Inter.Linear);
                        grayFace2 = grayFace2.Resize(100, 100, Inter.Linear);

                        // Comparar los rostros utilizando una métrica de similitud (por ejemplo, correlación)
                        //var result = grayFace1.Compare(grayFace2, CmpType.Correlation); Error

                        // Calcular el histograma de cada rostro
                        var hist1 = new UMat();
                        var hist2 = new UMat();

                        CvInvoke.CalcHist(new UMat[] { grayFace1 }, new int[] { 0 }, null, hist1, new Size(256, 1), new RangeF(0, 256));
                        CvInvoke.CalcHist(new UMat[] { grayFace2 }, new int[] { 0 }, null, hist2, new Size(256, 1), new RangeF(0, 256));

                        // Normalizar los histogramas
                        CvInvoke.Normalize(hist1, hist1, 0, 1, NormType.MinMax, DepthType.Cv32F);
                        CvInvoke.Normalize(hist2, hist2, 0, 1, NormType.MinMax, DepthType.Cv32F);*/

                        // Redimensionar los rostros a un tamaño común, usando UMat
                        Mat resizedFace1 = new Mat();
                        Mat resizedFace2 = new Mat();

                        CvInvoke.Resize(grayFace1, resizedFace1, new Size(1800, 1800));
                        CvInvoke.Resize(grayFace2, resizedFace2, new Size(1800, 1800));

                        // Comparar los histogramas utilizando correlación
                        var hist1 = new UMat();
                        var hist2 = new UMat();

                        VectorOfMat vou = new VectorOfMat();

                        vou.Push(resizedFace1);
                        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist1, new int[] { 256 }, new float[] { 0, 256 }, false);

                        vou = new VectorOfMat();

                        vou.Push(face2);
                        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist2, new int[] { 256 }, new float[] { 0, 256 }, false);

                        // Normalizar los histogramas
                        CvInvoke.Normalize(hist1, hist1, 0, 1, NormType.MinMax, DepthType.Cv32F);
                        CvInvoke.Normalize(hist2, hist2, 0, 1, NormType.MinMax, DepthType.Cv32F);

                        // Comparar los histogramas usando correlación
                        double correlation = CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Correl);

                        //Comparar con CompareFaces de FacialRecognition es mas exacta
                        bool facesAreSimilar = CompareFaces(resizedFace1, resizedFace2);

                        //Console.WriteLine($"Correlación entre los rostros: {correlation}");
                        resizedFace1.Save(@"1.jpg");
                        resizedFace2.Save(@"2.jpg");

                        if (correlation > 0.8)
                        { 
                            // Mostrar el resultado
                            CvInvoke.Imshow("Imagen", image1);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            CvInvoke.Imshow("Imagen", image2);
                            CvInvoke.WaitKey(0);
                            CvInvoke.DestroyAllWindows();

                            cRostro.status = "0";
                            cRostro.archivo = Path.GetFileName(archivo).Substring(0, Path.GetFileName(archivo).LastIndexOf("."));
                            cRostro.resultado = "La cara coincide con la del archivo: " + cRostro.archivo;
                        }
                    }
                    else
                    {
                        cRostro.status = "1";
                        cRostro.resultado = "No se detectaron rostros en la imágen guardada.";
                    }
                }
            }
            else
            {
                cRostro.status = "1";
                cRostro.resultado = "No se detectaron rostros en la imágen parámetro.";
            }
        }
        catch (Exception ex)
        {
            cRostro.status = "1";
            cRostro.resultado = ex.Message;
        }

        return cRostro;
    }

}