using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using static Emgu.CV.Stitching.Stitcher;
using static RestIOS.Clases.ValidaRostro;

class FacialRecognition
{
    private CascadeClassifier _faceCascade;
    private CascadeClassifier _eyeCascade;

    public FacialRecognition()
    {
        // Cargar el clasificador en cascada para el rostro
        _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
        _eyeCascade = new CascadeClassifier("haarcascade_eye.xml");
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

        try {
            // Convertir la cadena Base64 a un arreglo de bytes
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Escribir los bytes en un archivo de imagen
            uniqueName = "imgs/Temp/" + "image_" + Guid.NewGuid().ToString() + ".jpg";

            File.WriteAllBytes(uniqueName, imageBytes);

            Console.WriteLine("Imagen guardada en: " + uniqueName);
        }
        catch (Exception ex) {
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

        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist1,  new int[] { 256 }, new float[] { 0, 256 }, false);

        vou = new VectorOfMat();
        vou.Push(face2);

        CvInvoke.CalcHist(vou, new int[] { 0 }, null, hist2, new int[] { 256 }, new float[] { 0, 256 }, false);

        // Normalizar los histogramas
        CvInvoke.Normalize(hist1, hist1, 0, 255, NormType.MinMax);
        CvInvoke.Normalize(hist2, hist2, 0, 255, NormType.MinMax);

        // Comparar los histogramas
        double similarity = CvInvoke.CompareHist(hist1, hist2, HistogramCompMethod.Correl); //HistCompMethod 

        // Si la similitud es alta, asumimos que las caras son similares
        return similarity > 0.7; // Este valor puede ajustarse según el umbral deseado
    }

    public Crostro ValidaRostro(Mat imagenParametro, Mat imagenGuardada, string archivo)
    {
        Crostro cRostro = new Crostro();

        // Convertir la imagen a escala de grises
        UMat grayImage = new UMat();

        CvInvoke.CvtColor(imagenParametro, grayImage, ColorConversion.Bgr2Gray);

        // Detectar rostros en la imagen
        var faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 3, default, default);

        if (faces.Length > 0)
        {
            cRostro.status = "1";
            cRostro.resultado = "Ninguna cara coincide.";

            Mat faceImage1 = ExtractFace(imagenParametro, faces[0]);

            // Convertir la imagen a escala de grises
            CvInvoke.CvtColor(imagenGuardada, grayImage, ColorConversion.Bgr2Gray);

            // Detectar rostros en la imagen
            faces = _faceCascade.DetectMultiScale(grayImage, 1.1, 3, default, default);

            if (faces.Length > 0)
            {
                Mat faceImage2 = ExtractFace(imagenGuardada, faces[0]);

                // Comparar las caras
                bool facesAreSimilar = CompareFaces(faceImage1, faceImage2);

                if (facesAreSimilar)
                {
                    // Mostrar el resultado
                    CvInvoke.Imshow("Imagen", faceImage1);
                    CvInvoke.WaitKey(0);
                    CvInvoke.DestroyAllWindows();

                    CvInvoke.Imshow("Imagen", faceImage2);
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
        }
        else {
            cRostro.status = "1";
            cRostro.resultado = "No se detectaron rostros en la imágen parámetro.";
        }

        return cRostro; 
    }
}