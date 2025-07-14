import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function NotifyDrivers({ title, body, url }) {
  try {
    const res = await axiosUrl.post('push/notify-drivers', {
      title, body, url,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return 0;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function NotifyAll({ title, body, url }) {
  try {
    const res = await axiosUrl.post('push/notify-all', {
      title, body, url,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return 0;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}


export async function NotifyAdmin({ title, body, url }) {
  try {
    const res = await axiosUrl.post('push/notify-admin', {
      title, body, url,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return 0;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function NotifyClient({ orderId, title, body, url }) {
  try {
    const res = await axiosUrl.post('push/notify-one', {
      orderId, title, body, url,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return 0;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}
