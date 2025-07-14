//namespace Backend.Services
//{
//    using WebPush;
//    using System.Text.Json;
//    using Microsoft.AspNetCore.Mvc;
//    using Microsoft.EntityFrameworkCore;
//    using System.Security.Claims;
//    using Backend.DTO;
//    using Backend.Data;
//    using Microsoft.AspNetCore.Identity;
//    using System.Security.Policy;
//    using System;
//    using Humanizer;
//    using Microsoft.AspNetCore.SignalR;

//    public class PushService
//    {
//        private readonly string publicKey;
//        private readonly string privateKey;
//        private readonly AppDbContext context;
//        private readonly UserManager<IdentityUser> usermanager;

//        public PushService(AppDbContext _context, UserManager<IdentityUser> _usermanager)
//        {
//            publicKey = "BJLLSJE2MwoL6isV1uoS60VAh1GphYCZENkxD71icuwIoIm79hmbjpjNWcmO5qjOaqUZZLtiZ28Gs_xZZxUPN6A";
//            privateKey = "9xwEzK429KX7251MfNl4bNQhSN-GagrV4fkiCgFpSKY";
//            context = _context;
//            usermanager = _usermanager;
//        }

//        public async Task<List<string>> GetDrivers()
//        {
//            var result = new List<string>();
//            var users = await usermanager.Users.ToListAsync();
//            foreach (var user in users)
//            {
//                if (await usermanager.IsInRoleAsync(user, "Driver") == true)
//                {
//                    result.Add(user.Id!);
//                }
//            }
//            return result;
//        }

//        public async Task<List<string>> GetAdmins()
//        {
//            var result = new List<string>();
//            var users = await usermanager.Users.ToListAsync();
//            foreach (var user in users)
//            {
//                if (await usermanager.IsInRoleAsync(user, "Driver") == true)
//                {
//                    result.Add(user.Id!);
//                }
//            }
//            return result;
//        }
//        public async Task<List<string>> DriversInState(int stateId)
//        {
//            return await context.Drivers
//                .AsNoTracking()
//                .Where(d => d.WorkingState_id == stateId)
//                .Select(d => d.User_Id)  // ✅ Extract only UserId
//                .ToListAsync();
//        }
//        public async Task SendNotificationOnOrder(OrderDTO dto)
//        {
//            var webPushClient = new WebPushClient();
//            var vapidDetails = new VapidDetails(
//                "mailto:support.bnt@gmail.com",
//                "BJLLSJE2MwoL6isV1uoS60VAh1GphYCZENkxD71icuwIoIm79hmbjpjNWcmO5qjOaqUZZLtiZ28Gs_xZZxUPN6A",
//                "9xwEzK429KX7251MfNl4bNQhSN-GagrV4fkiCgFpSKY"
//            );

//            // Fetch all required data in ONE go
//            var drivers = await DriversInState(dto.StateId);

//            var address = await context.UserAddresses
//                .FirstOrDefaultAsync(a => a.Id == dto.UserAddress_Id); // Fetch address once
//            var subscriptions = await context.PushSubscriptions
//                .Where(sub => sub.OrderId == dto.Id || drivers.Contains(sub.UserId)) // Fetch Order and Driver Subscriptions
//                .ToListAsync();
//            var url = "https://wash-bg.com/profile";
//            var title = "Успешно направена поръчка №: " + dto.Id;
//            var body = $" Благодарим за поръчката Ви: {address.FullName}! \nОчаквайте обаждане на телефон: {address.Phone} " +
//                $" за потвърждение. \nЩе Ви изпратим съобщение при действие по поръчката!";

//            if (drivers.Contains(dto.UserId))
//            {
//                body = $" Адрес: {address.Address} \nИмена: {address.FullName} \nТелефон: {address.Phone} \nКоличество: {dto.QuantityS}м²";
//                title = "Нова поръчка №: " + dto.Id;
//                url = "https://wash-bg.com/driver";
//            }
//            if (dto.UserId == "23e40406-8a9d-2d82-912c-5d6a640ee697")
//            {
//                url = "";
//            }


//            if (address == null) return; // Handle missing address safely

//            var payload = JsonSerializer.Serialize(new
//            {
//                title = "Нова поръчка: " + dto.Id,
//                body = $" Адрес: {address.Address} \nИмена: {address.FullName} \nТелефон: {address.Phone} \nКоличество: {dto.QuantityS}м²",
//                icon = "/icon.png",
//                url = url
//            });

//            foreach (var sub in subscriptions)
//            {
//                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

//                try
//                {
//                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
//                }
//                catch (WebPushException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone)
//                {
//                    Console.WriteLine($"Push Error: {ex.Message}");
//                    context.PushSubscriptions.Remove(sub);
//                }
//            }

//            await context.SaveChangesAsync(); // ✅ Save changes once after loop
//        }
//        public async Task SendNotificationToDrivers(PayloadDTO dto)
//        {
//            var subscriptions = new List<Data.Models.PushSubscription>();
//            var drivers = await GetDrivers();
//            foreach (var id in drivers)
//            {
//                var driverSubscription = await context.PushSubscriptions.Where(s => s.UserId == id).ToListAsync();
//                subscriptions.AddRange(driverSubscription);
//            }


//            var webPushClient = new WebPushClient();
//            var vapidDetails = new VapidDetails(
//                "mailto:support.bnt@gmail.com",
//                "BJLLSJE2MwoL6isV1uoS60VAh1GphYCZENkxD71icuwIoIm79hmbjpjNWcmO5qjOaqUZZLtiZ28Gs_xZZxUPN6A",
//                "9xwEzK429KX7251MfNl4bNQhSN-GagrV4fkiCgFpSKY"
//            );

//            foreach (var sub in subscriptions)
//            {
//                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

//                var payload = JsonSerializer.Serialize(new
//                {
//                    title = dto.Title,
//                    body = dto.Body,
//                    icon = "/icon.png",
//                    url = dto.Url
//                });

//                try
//                {
//                    Console.WriteLine($"Sending notification: {dto.Body} to {subscriptions.Count} subscriptions");

//                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
//                }
//                catch (WebPushException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone)
//                {
//                    Console.WriteLine($"Push Error: {ex.Message}");
//                    context.PushSubscriptions.Remove(sub);
//                    await context.SaveChangesAsync();
//                }
//            }
//        }

//        public async Task SendNotificationToAdmin(PayloadDTO dto)
//        {
//            var subscriptions = new List<Data.Models.PushSubscription>();
//            var admins = await GetAdmins();
//            foreach (var id in admins)
//            {
//                var driverSubscription = await context.PushSubscriptions.Where(s => s.UserId == id).ToListAsync();
//                subscriptions.AddRange(driverSubscription);
//            }
           

//            var webPushClient = new WebPushClient();
//            var vapidDetails = new VapidDetails(
//                "mailto:support.bnt@gmail.com",
//                "BJLLSJE2MwoL6isV1uoS60VAh1GphYCZENkxD71icuwIoIm79hmbjpjNWcmO5qjOaqUZZLtiZ28Gs_xZZxUPN6A",
//                "9xwEzK429KX7251MfNl4bNQhSN-GagrV4fkiCgFpSKY"
//            );

//            foreach (var sub in subscriptions)
//            {
//                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

//                var payload = JsonSerializer.Serialize(new
//                { 
//                    title = dto.Title,
//                    body = dto.Body,
//                    icon = "/icon.png",
//                    url = dto.Url
//                });

//                try
//                {
//                    Console.WriteLine($"Sending notification: {dto.Body} to {subscriptions.Count} subscriptions");

//                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
//                }
//                catch (WebPushException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone)
//                {
//                    Console.WriteLine($"Push Error: {ex.Message}");
//                    context.PushSubscriptions.Remove(sub);
//                    await context.SaveChangesAsync();
//                }
//            }
//        }
//        public async Task SendNotificationAsync(PayloadDTO dto)
//        {
//            var subscriptions = await context.PushSubscriptions.ToListAsync();

//            var webPushClient = new WebPushClient();
//            var vapidDetails = new VapidDetails(
//                "mailto:support.bnt@gmail.com",
//                "BJLLSJE2MwoL6isV1uoS60VAh1GphYCZENkxD71icuwIoIm79hmbjpjNWcmO5qjOaqUZZLtiZ28Gs_xZZxUPN6A",
//                "9xwEzK429KX7251MfNl4bNQhSN-GagrV4fkiCgFpSKY"
//            );

//            foreach (var sub in subscriptions)
//            {
//                var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256dh, sub.Auth);

//                var payload = JsonSerializer.Serialize(new
//                {
//                    title = dto.Title,
//                    body = dto.Body,
//                    icon = "/icon.png",
//                    url = dto.Url
//                });

//                try
//                {
//                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
//                }
//                catch (WebPushException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone)
//                {
//                    context.PushSubscriptions.Remove(sub);
//                    await context.SaveChangesAsync();
//                }
//            }
//        }

//        public async Task SendNotificationToOneAsync(PayloadDTO dto)
//        {
//            var subscription = await context.PushSubscriptions
//                .Where(sub => sub.OrderId == dto.OrderId || sub.UserId == dto.UserId).FirstOrDefaultAsync();

//            if (subscription == null)
//            {
//                throw new Exception("No subscription found for this order.");
//            }

//            var webPushClient = new WebPushClient();
//            var vapidDetails = new VapidDetails(
//                "mailto:support.bnt@gmail.com",
//                publicKey,
//                privateKey
//            );

//            var pushSubscription = new PushSubscription(subscription.Endpoint, subscription.P256dh, subscription.Auth);

//            var payload = JsonSerializer.Serialize(new
//            {
//                title = dto.Title,
//                body = dto.Body,
//                icon = "/icon.png",
//                url = dto.Url
//            });

//            try
//            {
//                Console.WriteLine($"Sending notification: {dto.Body} to 1 subscriptions");
//                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
//            }
//            catch (WebPushException ex) when (ex.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Gone)
//            {
//                Console.WriteLine($"Push Error: {ex.Message}");
//                context.PushSubscriptions.Remove(subscription);
//                await context.SaveChangesAsync();
//            }
//        }


//        public async Task Subscribe(PushSubscriptionDto subscription, int orderId, string userId)
//        {
//            if (subscription == null)
//            {
//                throw new ArgumentNullException(nameof(subscription));
//            }

//            IQueryable<Data.Models.PushSubscription> query = context.PushSubscriptions.AsQueryable();

//            if (!string.IsNullOrEmpty(userId) && (orderId == 0 || orderId == null))
//            {
//                query = query.Where(sub => sub.UserId == userId && (sub.OrderId == 0 || sub.OrderId == null));
//            }
//            else
//            {
//                query = query.Where(sub => sub.OrderId == orderId);
//            }

//            var existingSubscription = await query.FirstOrDefaultAsync();

//            if (existingSubscription == null)
//            {
//                var newSub = new Data.Models.PushSubscription
//                {
//                    Endpoint = subscription.Endpoint,
//                    P256dh = subscription.P256dh,
//                    Auth = subscription.Auth,
//                    OrderId = orderId > 0 ? orderId : (int?)null, // Handle null OrderId properly
//                    UserId = !string.IsNullOrEmpty(userId) ? userId : "23e40406-8a9d-2d82-912c-5d6a640ee697"

//                };

//                context.PushSubscriptions.Add(newSub);
//                await context.SaveChangesAsync();
//            }
//        }



//        public async Task<bool> IsSubscriberOrder(int orderId)
//        {
//            return await context.PushSubscriptions.AnyAsync(sub => sub.OrderId == orderId);
//        }
//        public async Task<bool> IsSubscriberUser(string userId)
//        {
//            var res = await context.PushSubscriptions.AnyAsync(sub => sub.UserId == userId);
//            return res;
//        }

//        public async Task<List<Data.Models.PushSubscription>> AllSubs()
//        {
//            var res = await context.PushSubscriptions.AsNoTracking().ToListAsync();
//            return res;
//        }
//    }

//}
