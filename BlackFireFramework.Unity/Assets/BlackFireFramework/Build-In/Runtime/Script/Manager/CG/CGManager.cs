﻿//----------------------------------------------------
//Copyright © 2008-2018 Mr-Alan. All rights reserved.
//Mail: Mr.Alan.China@[outlook|gmail].com
//Website: www.0x69h.com
//----------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using UnityEngine;

namespace BlackFireFramework.Unity
{
    /// <summary>
    /// 人脸图像管家。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("BlackFire/CG")]
	public sealed partial class CGManager : ManagerBase
    {

        private void Init_FaceRecognizer()
        {
            
        }



        private IEnumerable<string> m_Paths;
        public void LoadFaceImages(IEnumerable<string> uris,Action loadCompleted=null)
        {
            m_Paths = uris;
            StartCoroutine(LoadFaceImagesYield(loadCompleted));
        }

        private List<Mat> m_Images = null;
        private MatOfInt m_Labels = null;
        private IEnumerator LoadFaceImagesYield(Action loadCompleted)
        {
            m_Images = new List<Mat>(); //OpenCV的图片列表。
            List<int> labelsList = new List<int>(); //图片的标签列表（整数）。
            m_Labels = new MatOfInt(); //标签列表（OpenCV内部的MatOfInt）
            int label = 0; //如果内部识别不出来，默认索引为0。
            foreach (var path in m_Paths)
            {
                m_Images.Add(Imgcodecs.imread(path, 0)); //通过OpenCV的IMRead读取添加OpenCV的图片。
                labelsList.Add (label++); //为图片打上标签。
                yield return null;
            }
            m_Labels.fromList(labelsList); //标签列表。
            
            if (null!=loadCompleted)
            {
                loadCompleted.Invoke();
            }
        }
        
        
        public FaceRecognitionEventArgs FaceRecognition(string sampleImageuri)
        {            
            Mat sampleMat = Imgcodecs.imread(sampleImageuri,0);
            int[] predictedLabel = new int[1]; //预判标签
            double[] predictedConfidence = new double[1]; //预判可信度

            BasicFaceRecognizer faceRecognizer = EigenFaceRecognizer.create();
            faceRecognizer.train(m_Images,m_Labels);
            faceRecognizer.predict (sampleMat,predictedLabel,predictedConfidence);
            
            Debug.Log ("Predicted class: " + predictedLabel[0] + " / " + "Actual class: " + 0);
            Debug.Log ("Confidence: " + predictedConfidence[0]);


            Mat predictedMat = m_Images[predictedLabel[0]];

            Mat baseMat = new Mat(sampleMat.rows(), predictedMat.cols() + sampleMat.cols(), CvType.CV_8UC1);
            predictedMat.copyTo(baseMat.submat (new OpenCVForUnity.Rect(0, 0, predictedMat.cols (), predictedMat.rows ())));
            sampleMat.copyTo(baseMat.submat (new OpenCVForUnity.Rect(predictedMat.cols (), 0, sampleMat.cols (), sampleMat.rows ())));

            Imgproc.putText(baseMat, "Predicted", new Point (10, 15), Core.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar (255), 1, Imgproc.LINE_AA, false);
            Imgproc.putText(baseMat, "Confidence:", new Point (5, 25), Core.FONT_HERSHEY_SIMPLEX, 0.2, new Scalar (255), 1, Imgproc.LINE_AA, false);
            Imgproc.putText(baseMat, "   " + predictedConfidence [0], new Point (5, 33), Core.FONT_HERSHEY_SIMPLEX, 0.2, new Scalar (255), 1, Imgproc.LINE_AA, false);
            Imgproc.putText(baseMat, "Sample", new Point (predictedMat.cols () + 10, 15), Core.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar (255), 1, Imgproc.LINE_AA, false);


            Texture2D texture = new Texture2D(baseMat.cols (), baseMat.rows (), TextureFormat.RGBA32, false);

            Utils.matToTexture2D(baseMat, texture);
            
            return new FaceRecognitionEventArgs()
            {
                PredictedLabel = predictedLabel[0],
                Confidence = predictedConfidence[0],
                FaceRecTexture = texture
            };
        }
        
    }


    public sealed class FaceRecognitionEventArgs
    {
        public int PredictedLabel; //预判等级
        public double Confidence; //可信度，也就是差异度
        public Texture2D FaceRecTexture; //对比后的图片
    }

}
