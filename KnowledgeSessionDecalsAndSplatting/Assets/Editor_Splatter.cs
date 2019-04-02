using UnityEngine;
using UnityEditor; // Dont forget to add this as we are extending the Editor
using System.Collections;

[CustomEditor(typeof(Splatter))] //Set tour script to extend the DoCake.cs
public class Editor_Splatter : Editor // Our script inherits from Editor
{
    // There is a variable called 'target' that comes from the Editor, its the script we are extending but to
    // make it easy to use we will decalre a new variable called '_target' that will cast this 'target' to our script type
    // otherwise you will need to cast it everytime you use it like this: int i = (ourType)target;

    Splatter _target;

    void OnEnable()
    {
        _target = (Splatter)target;
    }

    // Here is where the magic begins! You can use any GUI command here (As far as i know)
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("The splatter script", EditorStyles.boldLabel);



        _target.randomVelocityFactor = EditorGUILayout.FloatField("Spread of the particles", _target.randomVelocityFactor);
        
        _target.impactForce = EditorGUILayout.FloatField("The force the particle uses", _target.impactForce);

        _target.maxDecals = EditorGUILayout.IntField("How many particles wil spawn", _target.maxDecals);

        _target.particleLifeTime = EditorGUILayout.FloatField("Time until splatter is destroyed", _target.particleLifeTime);


        _target.reflectImpact = EditorGUILayout.Toggle("Reflect object on hit", _target.reflectImpact);

        if (_target.reflectImpact)
            _target.reflectStrength = EditorGUILayout.Slider("Amount of reflection", _target.reflectStrength, 0f, 1f);

        _target.particleToUse = (GameObject)EditorGUILayout.ObjectField("Splatter to be used", _target.particleToUse, typeof(GameObject), true);

        if (_target.particleToUse == null)
        {
            EditorGUILayout.HelpBox("You need to assign a particle you want to use!", MessageType.Error);
        }
        else
        {
            _target.minSplaterSize = EditorGUILayout.FloatField("Minimum particle size", _target.minSplaterSize);

            _target.maxSplaterSize = EditorGUILayout.FloatField("Maximum particle size", _target.maxSplaterSize);

            _target.decal = (GameObject)EditorGUILayout.ObjectField("Decal to be used", _target.decal, typeof(DeferredDecal), true);
            _target.decalMaterial = (Material)EditorGUILayout.ObjectField("Override decal material", _target.decalMaterial, typeof(Material), true);

            if (_target.decalMaterial == null)
            {
                EditorGUILayout.HelpBox("You need to assign a decal material you want to use!", MessageType.Error);
            }
            else
            {
                _target.randomYRotation = EditorGUILayout.Toggle("Random Y rotation for decal", _target.randomYRotation);

                _target.useNormalSurface = EditorGUILayout.Toggle("Normal surface rotation for decal", _target.useNormalSurface);

                _target.decalLifeTime = EditorGUILayout.FloatField("Time until decal is destroyed", _target.decalLifeTime);

                _target.decalStartFadeTime = EditorGUILayout.FloatField("Decal fade time", _target.decalStartFadeTime);

                _target.minDecalSize = EditorGUILayout.FloatField("Minimum decal size", _target.minDecalSize);

                _target.maxDecalSize = EditorGUILayout.FloatField("Maximum decal size", _target.maxDecalSize);
            }
        }



        GUILayout.EndVertical();

        //If we changed the GUI aply the new values to the script
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_target);
        }
    }
}