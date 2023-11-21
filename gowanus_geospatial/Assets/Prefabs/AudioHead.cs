using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.zibra.liquid.DataStructures;
using com.zibra.liquid.Solver;

public class AudioHead : MonoBehaviour
{

    public AudioSource audioSource;

    private ZibraLiquidMaterialParameters matParams;

    private ZibraLiquidSolverParameters solvParams;

    private float m_Hue;// = 123 / 360;
    private float m_Saturation;// = 100 / 100;
    private float m_Value;// = 0;




    public float updateStep = 0.1f;
	public int sampleDataLength = 1024;

	private float currentUpdateTime = 0f;

	private float clipLoudness;
	private float[] clipSampleData;



    private Color thisCol = new Color(0,0,0,1);


    // Start is called before the first frame update
    void Start()
    {
        //m_Hue = matParams.

        matParams = this.gameObject.GetComponent<ZibraLiquidMaterialParameters>();

        solvParams = this.gameObject.GetComponent<ZibraLiquidSolverParameters>();

        
        /*float H;
        float S;
        float V;
        Color.RGBToHSV(matParams.EmissiveColor, out H, out S, out V);

        m_Hue = H;
        m_Saturation = S;
        m_Value = V;*/


        //matParams.EmissiveColor = new Color(m_Hue, m_Saturation, m_Value, 1);
        //Color.HSVToRGB(m_Hue, m_Saturation, m_Value, true);

        clipSampleData = new float[sampleDataLength];


    }

    // Update is called once per frame
    void Update()
    {
        if(audioSource.isPlaying){


            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime >= updateStep) {
                currentUpdateTime = 0f;
                audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
                clipLoudness = 0f;
                foreach (var sample in clipSampleData) {
                    clipLoudness += Mathf.Abs(sample);
                }
                clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
            }

            //print(clipLoudness);

            //m_Value = 100 * clipLoudness / 4; //audioSource.volume;
            m_Saturation = 1.5f * clipLoudness; //audioSource.volume;

            //m_Value = 1000 * clipLoudness / 4; //audioSource.volume;
            

            //matParams.EmissiveColor = Color.HSVToRGB(m_Hue, m_Saturation, m_Value)*2;
            //(Color.HSVToRGB(122/360, 100/100, m_Value / 100))*2;

            matParams.EmissiveColor = (new Color(0, m_Saturation, 0, 1))*3;

            solvParams.MaximumVelocity = clipLoudness * 10 + 3;
            print(clipLoudness * 10 + 3);

            //matParams.EmissiveColor = (new Color(0, 0, m_Value, 1))*2;
            //matParams.EmissiveColor = (new Color(0, 0, 0, 1))*m_Value;
            //Color.HSVToRGB(m_Hue, m_Saturation, m_Value, true);

            //print(m_Hue);
            //print(m_Saturation);
            print(m_Value);

            

        }
    }
}
