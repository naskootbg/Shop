import { defineStore } from 'pinia';
 

export const useAdminStore = defineStore('adminStore', {
  state() {
    return { 
      orders: [],
      drivers: [],
      admins: [],
      driversInLocations: [],
    };
  },
  getters: {
     
  },
  actions: {
    async onEnter() {
  
       
      },
      
      
    },
    
   
  },
  
);
