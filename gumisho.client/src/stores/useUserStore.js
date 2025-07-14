import { defineStore } from 'pinia';
import { GetInfo, doLogin, doLogout } from '../api/identity';
import { ClearCard, RemoveProduct, AddProduct, CreateCard } from '../api/card';




export const useUserStore = defineStore('userStore', {
  state() {
    return {
      username: null,
      email: null,
      orders: [],
      adresses: [],
      roles: [],
      driveLocations: [],
      card: null,
    };
  },
  getters: {
    isUserLogged: s => s.email != null && s.username != null,
    haveAdresses: s => s.adresses != null,
    haveOrders: s => s.orders != null,
    isAdmin: s => s.roles.length > 0 && s.roles[0] === "Admin",
   
  },
  actions: {
    async onEnter() {
      const data = await GetInfo();

      this.username = data.userName;
      this.email = data.email || null;
      this.adresses = data.addresses || [];
      this.roles = data.role || [];
      this.orders = data.orders || [];

      const savedCard = JSON.parse(localStorage.getItem('card'));

      // 🧠 Make sure this.card exists first
      if (!this.card) {
        this.card = {}; // initialize if undefined
      }

      if (savedCard) {
        Object.assign(this.card, savedCard); // ✅ merge into reactive object
      }

      if (this.isUserLogged && (!savedCard || Object.keys(savedCard).length === 0)) {
        const newCard = await CreateCard(this.email, this.username);
        Object.assign(this.card, newCard); // ✅ merge for reactivity
        localStorage.setItem('card', JSON.stringify(this.card));
      }
    }
    ,
    async AddItem(code) {
      // Ensure card exists
      if (!this.card) {
        this.card = {};
      }

      // Ensure productCodes is an array
      if (!Array.isArray(this.card.productCodes)) {
        this.card.productCodes = [];
      }

      // Add code if not already present
      if (!this.card.productCodes.includes(code)) {
        this.card.productCodes.push(code);

        // Send to backend
        const newItem = {
          id: this.card.id,
          code: code
        };
        await AddProduct(newItem);

        // Save updated card back to localStorage
        localStorage.setItem('card', JSON.stringify(this.card));
      }
    }
,
    async RemoveItem(code) {
      // Make sure card and productCodes exist
      if (!this.card || !Array.isArray(this.card.productCodes)) return;

      // Remove the item (filter it out)
      this.card.productCodes = this.card.productCodes.filter(c => c !== code);

      // Update backend
      const payload = {
        id: this.card.id,
        code: code
      };
      await RemoveProduct(payload);

      // Save updated card to localStorage
      localStorage.setItem('card', JSON.stringify(this.card));
    }
,
    async ClearCard() {
      // Optional: Call backend to clear the card there too
      await ClearCard(this.card?.id); // only if card exists

      // Reset local state
      this.card = {
        id: this.card?.id || null,         // keep id if needed
        productCodes: []
      };

      // Update localStorage
      localStorage.setItem('card', JSON.stringify(this.card));
    }

,

      async LoginIn(loginData) {
        
        await doLogin(loginData);
        
           },
       
           async LogOut() {
            
            await doLogout();
            
               },
      
    },
    
   
  },
  
);
