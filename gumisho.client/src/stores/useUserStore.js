import { defineStore } from 'pinia';
import { GetInfo, doLogin, doLogout } from '../api/identity';
 



export const useUserStore = defineStore('userStore', {
  state() {
    return {
      username: null,
      email: null,
      orders: [],
      adresses: [],
      roles: [],
      driveLocations: [],
    };
  },
  getters: {
    isUserLogged: s => s.email != null,
    haveAdresses: s => s.adresses != null,
    haveOrders: s => s.orders != null,
    isAdmin: s => s.roles.length > 0 &&  s.roles[0] === "Admin",
  },
  actions: {
    async onEnter() {
       const data = await GetInfo();
      this.username = data.userName;
       this.email = data.email || null;
       this.adresses = data.addresses || [];
       this.roles = data.role || [];
      this.orders = data.orders || [];      
      
      },
      async LoginIn(loginData) {
        
        await doLogin(loginData);
        
           },
       
           async LogOut() {
            
            await doLogout();
            
               },
      
    },
    
   
  },
  
);
