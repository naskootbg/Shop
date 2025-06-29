import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function GetInfo(){
    try{
      const res = await axiosUrl.get('account/info');
      console.log(res.data);
return res.data;
    }
    catch (e){
logger.error(e);
return null;
    }
}


export async function doLogin({username, password}){
    try{
const res = await axiosUrl.post('account/login',{
    username,
    password
},{
    headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
});
      if (res.status == 200) {
        
 location.reload();

return true;
}

return false;
    }
    catch (e){
logger.error(e);
return[];
    }
}

export async function doLogout() {
    try{
        await axiosUrl.post('logout',{},{
            headers: {
                'accept': '*/*',
              },
        });


            }
            catch (e){
        logger.error(e);
        return[];
            }
        };

        export async function RegisterMe({username, password, email, phonenumber}){
            try{
        const res = await axiosUrl.post('account/register',{
            username,
            password,
            email,
            phonenumber
        },{
            headers: {
                'accept': 'application/json',
                'Content-Type': 'application/json',

              },
        });
        if(res.status == 200){
           // location.reload();
            return res.data;

        }

        return "";
            }
            catch (e){
        logger.error(e);
        return[];
            }
        }
