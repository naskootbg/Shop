<template>
  <button @click="handleSubscribe" :disabled="isSubscribed" class="subscribe-button">
    {{ isSubscribed ? "Subscribed" : "Subscribe to Notifications" }}
  </button>
</template>

<script setup>
  import { ref } from "vue";

  const isSubscribed = ref(false);

  async function handleSubscribe() {
    try {
      const permission = await Notification.requestPermission();
      if (permission !== "granted") {
        alert("You need to allow notifications!");
        return;
      }

      // Get service worker registration
      const registration = await navigator.serviceWorker.ready;

      // Get VAPID key from .env
      const applicationServerKey = urlBase64ToUint8Array(import.meta.env.VITE_PUBLIC_VAPID_KEY);

      // Subscribe to push notifications
      const subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey,
      });

      // Prepare data
      const pushData = {
        id: 0, // Auto-incremented in DB
        endpoint: subscription.endpoint,
        p256dh: encodeBase64(subscription.getKey("p256dh")),
        auth: encodeBase64(subscription.getKey("auth")),
        orderId: 0, // Modify if needed
        userId: "string" // Replace with actual user ID
      };

      // Send to backend
      await fetch("https://your-api.com/api/notifications/subscribe", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(pushData),
      });

      isSubscribed.value = true;
    } catch (error) {
      console.error("Subscription failed:", error);
    }
  }

  // Convert base64 URL-safe string
  function urlBase64ToUint8Array(base64String) {
    const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
    const base64 = (base64String + padding).replace(/-/g, "+").replace(/_/g, "/");
    const rawData = atob(base64);
    return new Uint8Array([...rawData].map((char) => char.charCodeAt(0)));
  }

  // Encode subscription keys
  function encodeBase64(buffer) {
    return btoa(String.fromCharCode(...new Uint8Array(buffer)));
  }
</script>

<style>
  .subscribe-button {
    padding: 10px 15px;
    font-size: 16px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }

    .subscribe-button:disabled {
      background-color: #6c757d;
      cursor: not-allowed;
    }
</style>
