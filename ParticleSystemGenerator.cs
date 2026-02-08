using UnityEngine;
using UnityEditor;

public class ParticleSystemGenerator : EditorWindow
{
    public enum ParticleType
    {
        Rain,
        RainSplash,
        Snow,
        DustCloud,
        Smoke,
        Fire,
        Fireball,
        Lightning,
        MagicSparkles,
        HealingAura,
        PoisonCloud,
        BloodSpray,
        Explosion,
        Confetti,
        Leaves,
        Fireflies,
        SteamVent,
        Waterfall,
        Embers,
        Sparks,
        ElectricArc,
        FrostBreath,
        ToxicGas,
        BubbleStream,
        SandStorm,
        Ash,
        Mist,
        Torch,
        MuzzleFlash,
        ShellCasings,
        ImpactDust,
        WaterRipple,
        MagicRunes,
        DarkEnergy,
        HolyLight,
        Footprints,
        // 18 New Mountain Biking particle systems
        MudSplatter,
        DirtKickup,
        RockDebris,
        TireDust,
        WaterSplash,
        PuddleSplash,
        GravelSpray,
        BrakeSmoke,
        ChainOil,
        BikeSkidMarks,
        TreeBranches,
        GrassCutting,
        PineCones,
        BirdScatter,
        BugSwarm,
        RainDroplets,
        FogBank,
        DustTrail
    }

    private ParticleType selectedType = ParticleType.Rain;
    private GameObject parentObject = null;
    private Material customMaterial = null;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Particle System Generator")]
    public static void ShowWindow()
    {
        GetWindow<ParticleSystemGenerator>("Particle Generator");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Particle System Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        selectedType = (ParticleType)EditorGUILayout.EnumPopup("Particle Type:", selectedType);
        
        GUILayout.Space(10);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object (Optional):", parentObject, typeof(GameObject), true);
        
        GUILayout.Space(10);
        customMaterial = (Material)EditorGUILayout.ObjectField("Custom Material (Optional):", customMaterial, typeof(Material), false);
        
        GUILayout.Space(10);
        if (GUILayout.Button("Create Particle System", GUILayout.Height(30)))
        {
            CreateParticleSystem();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Generate All Systems", GUILayout.Height(30)))
        {
            GenerateAllSystem();
        }

        // Display helpful info about the selected particle type
        GUILayout.Space(15);
        EditorGUILayout.HelpBox(GetParticleTypeInfo(selectedType), MessageType.Info);
        
        EditorGUILayout.EndScrollView();
    }
    
    string GetParticleTypeInfo(ParticleType type)
    {
        switch (type)
        {
            case ParticleType.Rain:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Use a stretched droplet texture with alpha transparency. Consider adding a slight blue tint and vertical stretching for realism.";
            case ParticleType.RainSplash:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small circular splash texture with radial expansion. White/light blue color works best with fade-out.";
            case ParticleType.Snow:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Soft white snowflake texture with subtle alpha. Small, gentle particles work best for realistic snowfall.";
            case ParticleType.DustCloud:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Soft, billowy cloud texture in tan/brown colors. Low alpha for realistic dust dispersal.";
            case ParticleType.Smoke:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Wispy smoke texture with soft edges. Gray colors with gradual alpha fade. Consider using noise/turbulence.";
            case ParticleType.Fire:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Flame texture with additive blending. Use gradient from yellow->orange->red for heat effect.";
            case ParticleType.Fireball:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Intense flame texture with additive blending. Bright orange/yellow center with particle trails for impact.";
            case ParticleType.Lightning:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Bright white/blue electric texture with high brightness. Stretched particles with additive blending for glow.";
            case ParticleType.MagicSparkles:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Star or sparkle texture with additive blending. Bright colors (white, purple, cyan) with glow effect.";
            case ParticleType.HealingAura:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Soft glow texture in green/cyan colors. Gentle additive blending with upward movement.";
            case ParticleType.PoisonCloud:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Toxic cloud texture in sickly green colors. Medium alpha for mysterious, dangerous effect.";
            case ParticleType.BloodSpray:
                return "DEFAULT SHADER: Particles/Multiply\n\nBEST MATERIAL: Splatter texture in dark red. Multiply blending makes it stick/stain. Consider droplet shapes.";
            case ParticleType.Explosion:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Fireball/smoke combo texture. Bright yellow->orange->dark gradient with rapid expansion.";
            case ParticleType.Confetti:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small rectangle/square with bright, varied colors. Rotation over lifetime for realistic falling.";
            case ParticleType.Leaves:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Leaf texture in autumn colors (orange, red, brown, yellow). Rotation and gentle falling motion.";
            case ParticleType.Fireflies:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Small glowing dot with soft glow. Yellow-green color with pulsing brightness over lifetime.";
            case ParticleType.SteamVent:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Wispy steam texture in white/light gray. Rising motion with expansion and alpha fade.";
            case ParticleType.Waterfall:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Water droplet or stream texture with blue-white tint. Fast downward movement with splash at bottom.";
            case ParticleType.Embers:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Glowing particle texture. Orange-red with gradual darkening and upward float.";
            case ParticleType.Sparks:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Bright streak or dot texture. Yellow-white color with trails. Fast initial velocity with gravity.";
            case ParticleType.ElectricArc:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Bright electric bolt texture. Blue-white with stretch rendering for lightning bolt effect.";
            case ParticleType.FrostBreath:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Icy mist texture in light blue-white. Cone shape emission with gradual dissipation.";
            case ParticleType.ToxicGas:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Poison cloud in green/yellow-green. Slowly spreading with medium opacity for dangerous atmosphere.";
            case ParticleType.BubbleStream:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Spherical bubble texture with rim highlight. Light blue-white with upward movement and size variation.";
            case ParticleType.SandStorm:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small sand grain texture in tan/beige. High velocity horizontal movement with lots of particles.";
            case ParticleType.Ash:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Irregular ash flake texture in gray/black. Slow upward float with gentle rotation.";
            case ParticleType.Mist:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Large soft fog texture. White/gray with very low opacity for atmospheric ground fog.";
            case ParticleType.Torch:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Flame texture with additive blending. Orange-yellow gradient with upward movement and flicker.";
            case ParticleType.MuzzleFlash:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Bright flash burst texture. Yellow-white with very short lifetime for gun firing effect.";
            case ParticleType.ShellCasings:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Metallic cylinder texture in brass/copper. Physics collision enabled with rotation for realism.";
            case ParticleType.ImpactDust:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Dust puff texture in gray-brown. Burst emission with outward expansion from impact point.";
            case ParticleType.WaterRipple:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Concentric circle ripple texture. Horizontal billboard with size expansion over lifetime.";
            case ParticleType.MagicRunes:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Mystical symbol/rune texture in purple/blue. Horizontal billboards with rotation and glow.";
            case ParticleType.DarkEnergy:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Swirling dark particle texture in purple/black with additive edges. Orbital movement recommended.";
            case ParticleType.HolyLight:
                return "DEFAULT SHADER: Particles/Additive\n\nBEST MATERIAL: Bright ray/beam texture in golden-white. Vertical emission with gentle swirl and glow.";
            case ParticleType.Footprints:
                return "DEFAULT SHADER: Particles/Multiply\n\nBEST MATERIAL: Footprint shape texture in brown/dark. Multiply blending to darken ground. Horizontal billboard.";
            case ParticleType.MudSplatter:
                return "DEFAULT SHADER: Particles/Multiply\n\nBEST MATERIAL: Splatter texture in brown mud color. Multiply blending for staining. Burst on collision/skid.";
            case ParticleType.DirtKickup:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Dirt cloud texture in brown. Continuous emission from rear wheel with backward trajectory.";
            case ParticleType.RockDebris:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small rock/pebble texture in gray-brown. Physics collision with rotation for bouncing rocks.";
            case ParticleType.TireDust:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Fine dust texture following tire path. Tan/gray color with low opacity and backward emission.";
            case ParticleType.WaterSplash:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Water droplet texture. Blue-white with burst emission when crossing streams/puddles.";
            case ParticleType.PuddleSplash:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Large water splash texture. Radial burst from puddle center with upward arc.";
            case ParticleType.GravelSpray:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small gravel stone texture. Gray with physics collision and rotation for realistic spray.";
            case ParticleType.BrakeSmoke:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Smoke puff texture in gray. Emit from brake area with upward drift during hard braking.";
            case ParticleType.ChainOil:
                return "DEFAULT SHADER: Particles/Multiply\n\nBEST MATERIAL: Small oil droplet texture in black. Multiply blending with drip/spray from chain area.";
            case ParticleType.BikeSkidMarks:
                return "DEFAULT SHADER: Particles/Multiply\n\nBEST MATERIAL: Tire tread mark texture in black. Horizontal billboard along ground with multiply for mark effect.";
            case ParticleType.TreeBranches:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small branch/twig texture in brown. Burst emission when hitting foliage with physics.";
            case ParticleType.GrassCutting:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Grass blade texture in green. Continuous emission from wheels cutting through grass.";
            case ParticleType.PineCones:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Pine cone texture in brown. Physics collision with tumbling rotation when knocked from tree.";
            case ParticleType.BirdScatter:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Bird silhouette in dark colors. Burst emission upward when rider approaches with rapid movement.";
            case ParticleType.BugSwarm:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small flying insect texture. Dark color with erratic movement pattern in swarm areas.";
            case ParticleType.RainDroplets:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Small water droplet texture. Emit around rider during rain with short lifetime (hitting rider/bike).";
            case ParticleType.FogBank:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Large volumetric fog texture in white/gray. Very low opacity for dense valley/forest fog.";
            case ParticleType.DustTrail:
                return "DEFAULT SHADER: Particles/Alpha Blended\n\nBEST MATERIAL: Dust cloud texture. Tan/brown following bike path with slow dissipation for visible trail.";
            default:
                return "Select a particle type to see material recommendations.";
        }
    }

    void CreateParticleSystem()
    {
        GameObject particleObj = new GameObject(selectedType.ToString());
        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        
        // Parent to assigned object if one exists
        if (parentObject != null)
        {
            particleObj.transform.SetParent(parentObject.transform);
            particleObj.transform.localPosition = Vector3.zero;
        }
        
        // Store whether user provided custom material
        bool hasCustomMaterial = customMaterial != null;
        
        switch (selectedType)
        {
            case ParticleType.Rain:
                SetupRain(ps, renderer);
                break;
            case ParticleType.RainSplash:
                SetupRainSplash(ps, renderer);
                break;
            case ParticleType.Snow:
                SetupSnow(ps, renderer);
                break;
            case ParticleType.DustCloud:
                SetupDustCloud(ps, renderer);
                break;
            case ParticleType.Smoke:
                SetupSmoke(ps, renderer);
                break;
            case ParticleType.Fire:
                SetupFire(ps, renderer);
                break;
            case ParticleType.Fireball:
                SetupFireball(ps, renderer);
                break;
            case ParticleType.Lightning:
                SetupLightning(ps, renderer);
                break;
            case ParticleType.MagicSparkles:
                SetupMagicSparkles(ps, renderer);
                break;
            case ParticleType.HealingAura:
                SetupHealingAura(ps, renderer);
                break;
            case ParticleType.PoisonCloud:
                SetupPoisonCloud(ps, renderer);
                break;
            case ParticleType.BloodSpray:
                SetupBloodSpray(ps, renderer);
                break;
            case ParticleType.Explosion:
                SetupExplosion(ps, renderer);
                break;
            case ParticleType.Confetti:
                SetupConfetti(ps, renderer);
                break;
            case ParticleType.Leaves:
                SetupLeaves(ps, renderer);
                break;
            case ParticleType.Fireflies:
                SetupFireflies(ps, renderer);
                break;
            case ParticleType.SteamVent:
                SetupSteamVent(ps, renderer);
                break;
            case ParticleType.Waterfall:
                SetupWaterfall(ps, renderer);
                break;
            case ParticleType.Embers:
                SetupEmbers(ps, renderer);
                break;
            case ParticleType.Sparks:
                SetupSparks(ps, renderer);
                break;
            case ParticleType.ElectricArc:
                SetupElectricArc(ps, renderer);
                break;
            case ParticleType.FrostBreath:
                SetupFrostBreath(ps, renderer);
                break;
            case ParticleType.ToxicGas:
                SetupToxicGas(ps, renderer);
                break;
            case ParticleType.BubbleStream:
                SetupBubbleStream(ps, renderer);
                break;
            case ParticleType.SandStorm:
                SetupSandStorm(ps, renderer);
                break;
            case ParticleType.Ash:
                SetupAsh(ps, renderer);
                break;
            case ParticleType.Mist:
                SetupMist(ps, renderer);
                break;
            case ParticleType.Torch:
                SetupTorch(ps, renderer);
                break;
            case ParticleType.MuzzleFlash:
                SetupMuzzleFlash(ps, renderer);
                break;
            case ParticleType.ShellCasings:
                SetupShellCasings(ps, renderer);
                break;
            case ParticleType.ImpactDust:
                SetupImpactDust(ps, renderer);
                break;
            case ParticleType.WaterRipple:
                SetupWaterRipple(ps, renderer);
                break;
            case ParticleType.MagicRunes:
                SetupMagicRunes(ps, renderer);
                break;
            case ParticleType.DarkEnergy:
                SetupDarkEnergy(ps, renderer);
                break;
            case ParticleType.HolyLight:
                SetupHolyLight(ps, renderer);
                break;
            case ParticleType.Footprints:
                SetupFootprints(ps, renderer);
                break;
            case ParticleType.MudSplatter:
                SetupMudSplatter(ps, renderer);
                break;
            case ParticleType.DirtKickup:
                SetupDirtKickup(ps, renderer);
                break;
            case ParticleType.RockDebris:
                SetupRockDebris(ps, renderer);
                break;
            case ParticleType.TireDust:
                SetupTireDust(ps, renderer);
                break;
            case ParticleType.WaterSplash:
                SetupWaterSplash(ps, renderer);
                break;
            case ParticleType.PuddleSplash:
                SetupPuddleSplash(ps, renderer);
                break;
            case ParticleType.GravelSpray:
                SetupGravelSpray(ps, renderer);
                break;
            case ParticleType.BrakeSmoke:
                SetupBrakeSmoke(ps, renderer);
                break;
            case ParticleType.ChainOil:
                SetupChainOil(ps, renderer);
                break;
            case ParticleType.BikeSkidMarks:
                SetupBikeSkidMarks(ps, renderer);
                break;
            case ParticleType.TreeBranches:
                SetupTreeBranches(ps, renderer);
                break;
            case ParticleType.GrassCutting:
                SetupGrassCutting(ps, renderer);
                break;
            case ParticleType.PineCones:
                SetupPineCones(ps, renderer);
                break;
            case ParticleType.BirdScatter:
                SetupBirdScatter(ps, renderer);
                break;
            case ParticleType.BugSwarm:
                SetupBugSwarm(ps, renderer);
                break;
            case ParticleType.RainDroplets:
                SetupRainDroplets(ps, renderer);
                break;
            case ParticleType.FogBank:
                SetupFogBank(ps, renderer);
                break;
            case ParticleType.DustTrail:
                SetupDustTrail(ps, renderer);
                break;
        }
        
        // Override with custom material if provided by user
        if (hasCustomMaterial)
        {
            renderer.material = customMaterial;
            Debug.Log("Created particle system: " + selectedType.ToString() + " with custom material: " + customMaterial.name);
        }
        else
        {
            Debug.Log("Created particle system: " + selectedType.ToString() + " with default shader");
        }
        
        Selection.activeGameObject = particleObj;
    }

    void GenerateAllSystem()
    {
        // Get all particle types from the enum
        ParticleType[] allTypes = (ParticleType[])System.Enum.GetValues(typeof(ParticleType));

        float xOffset = 0f;
        float spacing = 6f;

        // Create a parent object to hold all particle systems
        GameObject parentContainer = new GameObject("AllParticleSystems");
        if (parentObject != null)
        {
            parentContainer.transform.SetParent(parentObject.transform);
            parentContainer.transform.localPosition = Vector3.zero;
        }

        foreach (ParticleType type in allTypes)
        {
            GameObject particleObj = new GameObject(type.ToString());
            ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
            ParticleSystemRenderer renderer = particleObj.GetComponent<ParticleSystemRenderer>();

            // Parent to container
            particleObj.transform.SetParent(parentContainer.transform);
            particleObj.transform.localPosition = new Vector3(xOffset, 0, 0);

            // Store whether user provided custom material
            bool hasCustomMaterial = customMaterial != null;

            // Temporarily set selectedType to current type for the switch
            ParticleType originalSelection = selectedType;
            selectedType = type;

            switch (type)
            {
                case ParticleType.Rain:
                    SetupRain(ps, renderer);
                    break;
                case ParticleType.RainSplash:
                    SetupRainSplash(ps, renderer);
                    break;
                case ParticleType.Snow:
                    SetupSnow(ps, renderer);
                    break;
                case ParticleType.DustCloud:
                    SetupDustCloud(ps, renderer);
                    break;
                case ParticleType.Smoke:
                    SetupSmoke(ps, renderer);
                    break;
                case ParticleType.Fire:
                    SetupFire(ps, renderer);
                    break;
                case ParticleType.Fireball:
                    SetupFireball(ps, renderer);
                    break;
                case ParticleType.Lightning:
                    SetupLightning(ps, renderer);
                    break;
                case ParticleType.MagicSparkles:
                    SetupMagicSparkles(ps, renderer);
                    break;
                case ParticleType.HealingAura:
                    SetupHealingAura(ps, renderer);
                    break;
                case ParticleType.PoisonCloud:
                    SetupPoisonCloud(ps, renderer);
                    break;
                case ParticleType.BloodSpray:
                    SetupBloodSpray(ps, renderer);
                    break;
                case ParticleType.Explosion:
                    SetupExplosion(ps, renderer);
                    break;
                case ParticleType.Confetti:
                    SetupConfetti(ps, renderer);
                    break;
                case ParticleType.Leaves:
                    SetupLeaves(ps, renderer);
                    break;
                case ParticleType.Fireflies:
                    SetupFireflies(ps, renderer);
                    break;
                case ParticleType.SteamVent:
                    SetupSteamVent(ps, renderer);
                    break;
                case ParticleType.Waterfall:
                    SetupWaterfall(ps, renderer);
                    break;
                case ParticleType.Embers:
                    SetupEmbers(ps, renderer);
                    break;
                case ParticleType.Sparks:
                    SetupSparks(ps, renderer);
                    break;
                case ParticleType.ElectricArc:
                    SetupElectricArc(ps, renderer);
                    break;
                case ParticleType.FrostBreath:
                    SetupFrostBreath(ps, renderer);
                    break;
                case ParticleType.ToxicGas:
                    SetupToxicGas(ps, renderer);
                    break;
                case ParticleType.BubbleStream:
                    SetupBubbleStream(ps, renderer);
                    break;
                case ParticleType.SandStorm:
                    SetupSandStorm(ps, renderer);
                    break;
                case ParticleType.Ash:
                    SetupAsh(ps, renderer);
                    break;
                case ParticleType.Mist:
                    SetupMist(ps, renderer);
                    break;
                case ParticleType.Torch:
                    SetupTorch(ps, renderer);
                    break;
                case ParticleType.MuzzleFlash:
                    SetupMuzzleFlash(ps, renderer);
                    break;
                case ParticleType.ShellCasings:
                    SetupShellCasings(ps, renderer);
                    break;
                case ParticleType.ImpactDust:
                    SetupImpactDust(ps, renderer);
                    break;
                case ParticleType.WaterRipple:
                    SetupWaterRipple(ps, renderer);
                    break;
                case ParticleType.MagicRunes:
                    SetupMagicRunes(ps, renderer);
                    break;
                case ParticleType.DarkEnergy:
                    SetupDarkEnergy(ps, renderer);
                    break;
                case ParticleType.HolyLight:
                    SetupHolyLight(ps, renderer);
                    break;
                case ParticleType.Footprints:
                    SetupFootprints(ps, renderer);
                    break;
                case ParticleType.MudSplatter:
                    SetupMudSplatter(ps, renderer);
                    break;
                case ParticleType.DirtKickup:
                    SetupDirtKickup(ps, renderer);
                    break;
                case ParticleType.RockDebris:
                    SetupRockDebris(ps, renderer);
                    break;
                case ParticleType.TireDust:
                    SetupTireDust(ps, renderer);
                    break;
                case ParticleType.WaterSplash:
                    SetupWaterSplash(ps, renderer);
                    break;
                case ParticleType.PuddleSplash:
                    SetupPuddleSplash(ps, renderer);
                    break;
                case ParticleType.GravelSpray:
                    SetupGravelSpray(ps, renderer);
                    break;
                case ParticleType.BrakeSmoke:
                    SetupBrakeSmoke(ps, renderer);
                    break;
                case ParticleType.ChainOil:
                    SetupChainOil(ps, renderer);
                    break;
                case ParticleType.BikeSkidMarks:
                    SetupBikeSkidMarks(ps, renderer);
                    break;
                case ParticleType.TreeBranches:
                    SetupTreeBranches(ps, renderer);
                    break;
                case ParticleType.GrassCutting:
                    SetupGrassCutting(ps, renderer);
                    break;
                case ParticleType.PineCones:
                    SetupPineCones(ps, renderer);
                    break;
                case ParticleType.BirdScatter:
                    SetupBirdScatter(ps, renderer);
                    break;
                case ParticleType.BugSwarm:
                    SetupBugSwarm(ps, renderer);
                    break;
                case ParticleType.RainDroplets:
                    SetupRainDroplets(ps, renderer);
                    break;
                case ParticleType.FogBank:
                    SetupFogBank(ps, renderer);
                    break;
                case ParticleType.DustTrail:
                    SetupDustTrail(ps, renderer);
                    break;
            }

            // Override with custom material if provided by user
            if (hasCustomMaterial)
            {
                renderer.material = customMaterial;
            }

            // Restore original selection
            selectedType = originalSelection;

            // Move to next position
            xOffset += spacing;
        }

        Selection.activeGameObject = parentContainer;
        Debug.Log("Created all " + allTypes.Length + " particle systems in a row!");
    }

    // Helper function to load material from Assets folder or create one with shader
    Material GetOrCreateMaterial(string shaderPath, ParticleType particleType)
    {
        // Try to load material from Assets/Particle_Materials/ folder
        string materialPath = "Assets/Particle_Materials/" + particleType.ToString() + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

        if (mat != null)
        {
            return mat;
        }

        // If material doesn't exist, create one with the specified shader
        Shader shader = Shader.Find(shaderPath);
        if (shader != null)
        {
            return new Material(shader);
        }

        // Fallback to default particle shader
        return new Material(Shader.Find("Particles/Alpha Blended"));
    }

    // Add this helper function to your class
    void AddSecondaryLayer(ParticleSystem parentPS, string layerName, string shaderPath, ParticleType particleType)
    {
        GameObject secondaryObj = new GameObject(layerName);
        secondaryObj.transform.SetParent(parentPS.transform);
        secondaryObj.transform.localPosition = Vector3.zero;

        ParticleSystem secondaryPS = secondaryObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer secondaryRenderer = secondaryObj.GetComponent<ParticleSystemRenderer>();

        // Copy main settings from parent
        var parentMain = parentPS.main;
        var secondaryMain = secondaryPS.main;
        secondaryMain.startLifetime = parentMain.startLifetime;
        secondaryMain.startSpeed = parentMain.startSpeed;
        secondaryMain.startSize = new ParticleSystem.MinMaxCurve(
            parentMain.startSize.constant * 1.2f,
            parentMain.startSize.constantMax * 1.2f
        );
        secondaryMain.startColor = new Color(1f, 1f, 1f, 0.3f);
        secondaryMain.gravityModifier = parentMain.gravityModifier;

        // Copy emission
        var secondaryEmission = secondaryPS.emission;
        secondaryEmission.rateOverTime = parentPS.emission.rateOverTime.constant * 0.7f;

        // Copy shape
        var parentShape = parentPS.shape;
        var secondaryShape = secondaryPS.shape;
        secondaryShape.shapeType = parentShape.shapeType;
        secondaryShape.angle = parentShape.angle * 1.5f;
        secondaryShape.radius = parentShape.radius * 1.2f;

        // Apply material
        secondaryRenderer.material = GetOrCreateMaterial(shaderPath, particleType);

    }

    // Original 18 particle systems
    void SetupRain(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = 10f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.7f, 0.8f, 1f, 0.6f);
        main.gravityModifier = 2f;
        main.maxParticles = 1000;

        var emission = ps.emission;
        emission.rateOverTime = 500;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(10, 0.1f, 10);
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }

    void SetupRainSplash(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.7f);
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 5, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;
        shape.radiusThickness = 1f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupSnow(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 10f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startColor = Color.white;
        main.gravityModifier = 0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 50;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(20, 0.1f, 20);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupDustCloud(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startColor = new Color(0.8f, 0.7f, 0.6f, 0.3f);

        var emission = ps.emission;
        emission.rateOverTime = 20;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 2));

        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupSmoke(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.5f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(2f, 2f);

        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 2));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.gray, 0f), new GradientColorKey(Color.gray, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupFire(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startColor = new Color(1f, 0.5f, 0f, 1f);
        main.gravityModifier = -0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 50;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 10f;
        shape.radius = 0.3f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f),
                new GradientColorKey(new Color(1f, 0f, 0f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupFireball(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(1f, 0.3f, 0f, 1f);

        var emission = ps.emission;
        emission.rateOverTime = 100;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0.5f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var trails = ps.trails;
        trails.enabled = true;
        trails.lifetime = 0.3f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        renderer.trailMaterial = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupLightning(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 15f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.8f, 0.8f, 1f, 1f);
        main.maxParticles = 50;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20, 30) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.1f;
        shape.length = 5f;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 2f;
        renderer.velocityScale = 0.5f;  // ADD THIS - controls stretch based on velocity
        renderer.cameraVelocityScale = 0f; // ADD THIS - prevents camera movement stretch

        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupMagicSparkles(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 0.8f, 1f, 1f);

        var emission = ps.emission;
        emission.rateOverTime = 50;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 1f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 1f), 0.5f),
                new GradientColorKey(new Color(0.5f, 0.5f, 1f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Sparkle", ParticleType.Fireflies);
    }
    void SetupHealingAura(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0f, 1f, 0.5f, 0.7f);
        main.gravityModifier = -0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;
        shape.radiusThickness = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 1f), 0f),
                new GradientColorKey(new Color(0f, 1f, 0.5f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.7f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupPoisonCloud(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startColor = new Color(0.3f, 0.8f, 0.2f, 0.4f);

        var emission = ps.emission;
        emission.rateOverTime = 25;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1.5f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.5f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.3f, 0.8f, 0.2f), 0f), new GradientColorKey(new Color(0.2f, 0.5f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.5f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupBloodSpray(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startColor = new Color(0.6f, 0f, 0f, 1f);
        main.gravityModifier = 2f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30, 50) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.8f, 0f, 0f), 0f), new GradientColorKey(new Color(0.4f, 0f, 0f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupExplosion(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(10f, 20f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 1f);
        main.startColor = new Color(1f, 0.5f, 0f, 1f);
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 100, 150) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0.5f), 0f),
                new GradientColorKey(new Color(1f, 0.3f, 0f), 0.3f),
                new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 1, 1, 0));
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupConfetti(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50, 100) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-180f, 180f);
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Sparkle", ParticleType.Fireflies);
    }
    void SetupLeaves(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.4f, 0.6f, 0.2f, 1f);
        main.gravityModifier = 0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 10;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(10, 5, 10);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, 1f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-1f, 1f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-90f, 90f);
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupFireflies(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startColor = new Color(1f, 1f, 0.5f, 1f);

        var emission = ps.emission;
        emission.rateOverTime = 20;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(5, 3, 5);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 1f, 0.5f), 0f), new GradientColorKey(new Color(0.5f, 1f, 0.5f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(1f, 0.5f), new GradientAlphaKey(0.5f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Sparkle", ParticleType.Fireflies);
    }
    void SetupSteamVent(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startColor = new Color(1f, 1f, 1f, 0.6f);
        main.gravityModifier = -0.2f;

        var emission = ps.emission;
        emission.rateOverTime = 40;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 5f;
        shape.radius = 0.2f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 2));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupWaterfall(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = 8f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.7f);
        main.gravityModifier = 3f;

        var emission = ps.emission;
        emission.rateOverTime = 200;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(2, 0.1f, 0.5f);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
        
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }

    // 18 New particle systems
    void SetupEmbers(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 0.4f, 0f, 1f);
        main.gravityModifier = -0.2f;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 20f;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.7f, 0.3f), 0f),
                new GradientColorKey(new Color(1f, 0.2f, 0f), 0.7f),
                new GradientColorKey(new Color(0.3f, 0.1f, 0.1f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 1, 1, 0.2f));
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupSparks(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(1f, 0.8f, 0.3f, 1f);
        main.gravityModifier = 1.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20, 40) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
        shape.radius = 0.1f;

        var trails = ps.trails;
        trails.enabled = true;
        trails.lifetime = 0.2f;
        trails.minVertexDistance = 0.1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0.8f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        renderer.trailMaterial = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupElectricArc(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.15f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 15f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.5f, 0.8f, 1f, 1f);

        var emission = ps.emission;
        emission.rateOverTime = 100;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 5f;
        shape.radius = 0.05f;
        shape.length = 3f;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 3f;
        renderer.velocityScale = 0.1f;  
        renderer.cameraVelocityScale = 0f; 

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 1f, 1f), 0f), new GradientColorKey(new Color(0.3f, 0.6f, 1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupFrostBreath(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.7f, 0.9f, 1f, 0.6f);

        var emission = ps.emission;
        emission.rateOverTime = 50;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 20f;
        shape.radius = 0.3f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, 1f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);

        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.5f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(1f, 1f, 1f), 0f), new GradientColorKey(new Color(0.5f, 0.8f, 1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupToxicGas(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.5f);
        main.startColor = new Color(0.5f, 0.8f, 0.1f, 0.5f);
        main.gravityModifier = -0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 20;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.5f, 0.5f);

        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 0.7f, 1, 1.3f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.7f, 1f, 0.2f), 0f), new GradientColorKey(new Color(0.3f, 0.6f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.6f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupBubbleStream(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.4f);
        main.startColor = new Color(0.8f, 0.9f, 1f, 0.5f);
        main.gravityModifier = -0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 15;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 10f;
        shape.radius = 0.2f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 0.5f, 1, 1.2f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.7f, 0.9f, 1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.3f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupSandStorm(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(0.8f, 0.7f, 0.5f, 0.5f);

        var emission = ps.emission;
        emission.rateOverTime = 100;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(0.5f, 5, 10);
        shape.rotation = new Vector3(0, 0, 90);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-1f, 1f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-2f, 2f);
        
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupAsh(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.1f);
        main.startColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);
        main.gravityModifier = -0.05f;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(10, 0.5f, 10);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 0.4f, 0.4f), 0f), new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.6f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupMist(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 10f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startColor = new Color(0.9f, 0.95f, 1f, 0.2f);

        var emission = ps.emission;
        emission.rateOverTime = 5;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(20, 0.5f, 20);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 0.8f, 1, 1.2f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.9f, 0.95f, 1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.3f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupTorch(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(1f, 0.6f, 0.2f, 1f);
        main.gravityModifier = -0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 40;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 8f;
        shape.radius = 0.15f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.8f, 0.3f), 0f),
                new GradientColorKey(new Color(1f, 0.4f, 0f), 0.5f),
                new GradientColorKey(new Color(0.5f, 0f, 0f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 1, 1, 0.3f));
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupMuzzleFlash(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startColor = new Color(1f, 0.9f, 0.7f, 1f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 5, 10) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 0.8f), 0f),
                new GradientColorKey(new Color(1f, 0.5f, 0.2f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupShellCasings(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.08f);
        main.startColor = new Color(0.7f, 0.6f, 0.3f, 1f);
        main.gravityModifier = 2f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1, 3) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.05f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.x = new ParticleSystem.MinMaxCurve(200f, 400f);
        rotation.y = new ParticleSystem.MinMaxCurve(100f, 200f);
        rotation.z = new ParticleSystem.MinMaxCurve(200f, 400f);

        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.bounce = 0.5f;
        collision.lifetimeLoss = 0.3f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupImpactDust(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
        main.startColor = new Color(0.7f, 0.6f, 0.5f, 0.7f);
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15, 25) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.5f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.5f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.8f, 0.7f, 0.6f), 0f), new GradientColorKey(new Color(0.5f, 0.4f, 0.3f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupWaterRipple(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 0f;
        main.startSize = 0.5f;
        main.startColor = new Color(0.7f, 0.85f, 1f, 0.6f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 3, 5) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 3f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.9f, 0.95f, 1f), 0f), new GradientColorKey(new Color(0.6f, 0.8f, 1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupMagicRunes(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startColor = new Color(0.7f, 0.5f, 1f, 0.8f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 5;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 1.5f;
        shape.radiusThickness = 1f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-45f, 45f);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.8f, 1f), 0f),
                new GradientColorKey(new Color(0.5f, 0.3f, 1f), 0.5f),
                new GradientColorKey(new Color(0.3f, 0.5f, 1f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupDarkEnergy(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.3f, 0f, 0.5f, 0.8f);
        main.gravityModifier = -0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 40;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;
        shape.radiusThickness = 0.3f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-2f, 2f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-2f, 2f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-2f, 2f);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(0.5f, 0f, 0.8f), 0f),
                new GradientColorKey(new Color(0.2f, 0f, 0.4f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.3f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var trails = ps.trails;
        trails.enabled = true;
        trails.lifetime = 0.5f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
        renderer.trailMaterial = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupHolyLight(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(1f, 1f, 0.8f, 0.9f);
        main.gravityModifier = -0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 0f;
        shape.radius = 0.5f;
        shape.length = 5f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, 1f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-1f, 1f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 1f, 1f), 0f),
                new GradientColorKey(new Color(1f, 1f, 0.7f), 0.5f),
                new GradientColorKey(new Color(1f, 0.9f, 0.6f), 1f)
            },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/AdditiveGlow", ParticleType.Fire);
    }
    void SetupFootprints(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 5f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.4f);
        main.startColor = new Color(0.3f, 0.25f, 0.2f, 0.6f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.01f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 0.3f, 0.25f), 0f), new GradientColorKey(new Color(0.2f, 0.15f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }

    void SetupMudSplatter(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.3f, 0.2f, 0.1f, 0.8f);
        main.gravityModifier = 2f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10, 20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
        shape.radius = 0.3f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 0.25f, 0.15f), 0f), new GradientColorKey(new Color(0.25f, 0.15f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.3f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupDirtKickup(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startColor = new Color(0.5f, 0.4f, 0.3f, 0.6f);
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 0.2f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.5f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.6f, 0.5f, 0.4f), 0f), new GradientColorKey(new Color(0.4f, 0.3f, 0.2f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupRockDebris(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        main.gravityModifier = 2.5f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 5, 15) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 40f;
        shape.radius = 0.2f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-360f, 360f);
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupTireDust(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.6f, 0.55f, 0.5f, 0.5f);
        main.gravityModifier = 0.3f;

        var emission = ps.emission;
        emission.rateOverTime = 40;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.3f;
        shape.rotation = new Vector3(-90, 0, 0);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, 1f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.2f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.7f, 0.65f, 0.6f), 0f), new GradientColorKey(new Color(0.5f, 0.45f, 0.4f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupWaterSplash(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startColor = new Color(0.6f, 0.7f, 0.8f, 0.7f);
        main.gravityModifier = 2f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15, 30) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 50f;
        shape.radius = 0.2f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.7f, 0.8f, 0.9f), 0f), new GradientColorKey(new Color(0.5f, 0.6f, 0.7f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0.2f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupPuddleSplash(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.6f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.5f, 0.6f, 0.7f, 0.6f);
        main.gravityModifier = 1.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20, 40) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.4f;
        shape.radiusThickness = 0.5f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(3f, 6f);
        
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupGravelSpray(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.08f);
        main.startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        main.gravityModifier = 2f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 10, 25) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
        shape.radius = 0.2f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-720f, 720f);

        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.bounce = 0.6f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupBrakeSmoke(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);

        var emission = ps.emission;
        emission.rateOverTime = 25;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1f, 1f);

        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.5f, 1, 1.5f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 0.4f, 0.4f), 0f), new GradientColorKey(new Color(0.2f, 0.2f, 0.2f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
        // Add heat distortion layer
        AddSecondaryLayer(ps, "HeatDistortion", "Custom/Particles/HeatDistortion", ParticleType.SteamVent);
    }
    void SetupChainOil(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        main.gravityModifier = 2f;

        var emission = ps.emission;
        emission.rateOverTime = 15;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.15f, 0.15f, 0.15f), 0f), new GradientColorKey(new Color(0.1f, 0.1f, 0.1f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.3f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupBikeSkidMarks(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.6f);
        main.startColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 20;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.25f, 0.25f, 0.25f), 0f), new GradientColorKey(new Color(0.15f, 0.15f, 0.15f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // renderer already passed as parameter
        renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupTreeBranches(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startColor = new Color(0.3f, 0.2f, 0.1f, 1f);
        main.gravityModifier = 1f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 3, 8) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.3f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-180f, 180f);
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupGrassCutting(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.startColor = new Color(0.3f, 0.6f, 0.2f, 0.8f);
        main.gravityModifier = 1.5f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 40f;
        shape.radius = 0.2f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-360f, 360f);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.4f, 0.7f, 0.3f), 0f), new GradientColorKey(new Color(0.2f, 0.4f, 0.15f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.3f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupPineCones(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startColor = new Color(0.4f, 0.3f, 0.2f, 1f);
        main.gravityModifier = 2f;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 1, 3) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 20f;
        shape.radius = 0.1f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.x = new ParticleSystem.MinMaxCurve(-180f, 180f);
        rotation.z = new ParticleSystem.MinMaxCurve(-180f, 180f);

        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.bounce = 0.3f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupBirdScatter(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
        main.startColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        main.gravityModifier = -0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 3, 6) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 1f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-2f, 2f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(2f, 5f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-2f, 2f);

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0.5f));
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Debris", ParticleType.RockDebris);
    }
    void SetupBugSwarm(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 10f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.startColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var emission = ps.emission;
        emission.rateOverTime = 30;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 2f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-1f, 1f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-1f, 1f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.5f;
        noise.frequency = 1f;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/Sparkle", ParticleType.Fireflies);
    }
    void SetupRainDroplets(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 0.3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.1f);
        main.startColor = new Color(0.7f, 0.8f, 0.9f, 0.6f);
        main.gravityModifier = 1f;

        var emission = ps.emission;
        emission.rateOverTime = 50;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.8f, 0.9f, 1f), 0f), new GradientColorKey(new Color(0.6f, 0.7f, 0.8f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0.2f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/WaterDroplet", ParticleType.Rain);
    }
    void SetupFogBank(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 15f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startColor = new Color(0.85f, 0.9f, 0.95f, 0.3f);

        var emission = ps.emission;
        emission.rateOverTime = 3;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(15, 2, 15);

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.3f, 0.3f);

        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0, 0.7f, 1, 1.2f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(0.85f, 0.9f, 0.95f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.4f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
    void SetupDustTrail(ParticleSystem ps, ParticleSystemRenderer renderer)
    {
        var main = ps.main;
        main.startLifetime = 2.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 1f);
        main.startColor = new Color(0.6f, 0.55f, 0.5f, 0.4f);
        main.gravityModifier = -0.1f;

        var emission = ps.emission;
        emission.rateOverTime = 25;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.3f, 1f);

        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f, 0f);
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 0.6f, 1, 1.3f));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(new Color(0.7f, 0.65f, 0.6f), 0f), new GradientColorKey(new Color(0.5f, 0.45f, 0.4f), 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        renderer.material = GetOrCreateMaterial("Custom/Particles/SoftCloud", ParticleType.Smoke);
    }
}
