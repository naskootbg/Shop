import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function AllDrivers(){
    try{
const res = await axiosUrl.get('driver/all');
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}

export async function MyStates() {
  try {
    const res = await axiosUrl.get('driver/my-state');
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetDriverOrders(stateId){
    try{
const res = await axiosUrl.get('driver/orders?stateId=' + stateId);
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }  
}


export async function UpdateOrderStatus({orderId, statusId}){
    try{
      const res = await axiosUrl.post(`driver/update-status?$orderId=${orderId}&statusId=${statusId}`,{
   
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}

export async function SendToOtherDriver({orderId, stateId}){
    try{
const res = await axiosUrl.post('driver/change-driver',{
    orderId,
    stateId
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}

export async function SetDeliveryDate({orderId, date}){
    try{
const res = await axiosUrl.post('driver/set-date',{
    orderId,
    date
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
        
      },
});
if(res.status = 200){
 
return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}
