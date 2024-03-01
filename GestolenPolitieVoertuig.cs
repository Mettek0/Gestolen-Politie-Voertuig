using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API.Utils;
using FivePD.API;

namespace CalloutPack_Mettek0
{
    [CalloutProperties(name:"Gestolen Politie Auto", author:"Mettek0", version:"1.0.0")]
    public class GestolenPolitieVoertuig : Callout
    {
        private Vehicle car;
        private Ped suspect;
        
        Random random = new Random();
        float offsetX = new Random().Next(100, 700);
        float offsetY = new Random().Next(100, 700);
        
        

        public GestolenPolitieVoertuig()
        {
            InitInfo(
                World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, z: 0))));
            ShortName = "Gestolen Politie Auto";
            CalloutDescription = "Er is een politie voertuig gestolen";
            ResponseCode = 3;
            StartDistance = 200f;
        }

        private WeaponHash getRandomWeapon()
        {
            List<WeaponHash> weapons = new List<WeaponHash>
            {
                WeaponHash.Pistol,
                WeaponHash.Bat,
                WeaponHash.Dagger,
                WeaponHash.SwitchBlade,
                WeaponHash.Crowbar,
                WeaponHash.Unarmed
            };
            return weapons[random.Next(weapons.Count)];
        }

        private async Task Rijd_Weg()
        {
            // rijd weg
            API.SetDriveTaskDrivingStyle(suspect.GetHashCode(), 787004);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);
            Pursuit.RegisterPursuit(suspect);
        }


        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location + 1);
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            car = await CreateRandomVehicle(Location + 1);
            suspect.SetIntoVehicle(car, VehicleSeat.Driver);
            int tijd = random.Next(10000, 50000);
            int chance = random.Next(1, 6);
            if (chance >= 1 && chance <= 3)
            {
                // rijd weg
                API.SetDriveTaskDrivingStyle(suspect.GetHashCode(), 787004);
                Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);
                Pursuit.RegisterPursuit(suspect);
            } else if (chance >= 4 && chance <= 5)
            {
                // te voet weg lopen
                API.SetDriveTaskDrivingStyle(suspect.GetHashCode(), 787004);
                Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);
                Pursuit.RegisterPursuit(suspect);
                await BaseScript.Delay(tijd);
                suspect.Task.LeaveVehicle();
                suspect.Task.FleeFrom(player);
            } else if (chance == 6)
            {
                // stap uit en schiet / vecht
                API.SetDriveTaskDrivingStyle(suspect.GetHashCode(), 787004);
                Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId, true);
                Pursuit.RegisterPursuit(suspect);
                suspect.Weapons.Give(getRandomWeapon(), 99, true, true);
                await BaseScript.Delay(tijd);
                suspect.Task.LeaveVehicle();
                suspect.Task.FightAgainst(player);
            }

            
            
            
            
            
            
            //suspect.AttachBlip(); 
            //car.AttachBlip();

            

            
        }
        
        
        
        
        
        
        
        
        public async override Task OnAccept()
        {
            
            InitBlip();
            UpdateData();
        }

        public async Task<Vehicle> CreateRandomVehicle(Vector3 Location)
        {
            List<VehicleHash> vehicleModels = new List<VehicleHash>()
            { 
                (VehicleHash)1323650883,  // BC_T61
                (VehicleHash)933804727, // BC_Caddy
                (VehicleHash)1949606480, // BC_Motor
                (VehicleHash)739260930, // BC_Tiguan
                (VehicleHash)174731779, // LS_330i
                (VehicleHash)699354107, // LS_T61
                (VehicleHash)2112802830, // LS_Motor
                (VehicleHash)1037101579, // LS_Octavia
                (VehicleHash)1662264192, // LS_Tiguan
                (VehicleHash)2023869742, // LS_Vito
                (VehicleHash)1398415465 //LS_XC90
            };
            VehicleHash randomVehicle = vehicleModels[random.Next(vehicleModels.Count)];

            return await SpawnVehicle(randomVehicle, base.Location + 1);
        }

    }
}