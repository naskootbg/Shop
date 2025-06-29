import axiosOne from "@/config/axiosOne";
import { logger } from "@/utils/logger";

export async function AddUser({ test_external_id, email, phone}) {
  try {
    const res = await axiosOne.post('users', {
      properties: {
        language: 'bg',
        timezone_id: 'America\/Los_Angeles',
        lat: 90,
        long: 135,
        country: 'BG',
        first_active: 1678215680,
        last_active: 1678215682
      },
      identity: { external_id: test_external_id },
      subscriptions: [
        { type: 'WindowsPush', token: email, enabled: true },
        { type: 'Email', token: email, enabled: true },
        { type: 'ChromePush', token: email, enabled: true },
        { type: 'AndroidPush', token: email, enabled: true },
        { type: 'FirefoxPush', token: email, enabled: true },
        { type: 'SafariPush', token: email, enabled: true },
        { type: 'macOSPush', token: email, enabled: true },
        { type: 'HuaweiPush', token: email, enabled: true },
        { type: 'SMS', token: phone },
      ]   

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status = 200) {
      console.log(res.data);
      return true;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
} 
