import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function ShowOrder(id) {
  try {
    const res = await axiosUrl.get('order/show?id=' + id);
    return res.data;
  }
  catch (e) {
    logger.error(e);
    return null;
  }
}


export async function OrderSearch({ q }) {
  try {
    const res = await axiosUrl.post('order/search?q='+q, {
 
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function OrderSearchId({ id }) {
  try {
    const res = await axiosUrl.post('order/search-id?id=' + id, {

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return res.data;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function CancelOrder({ id }) {
  try {
    const res = await axiosUrl.post('order/cancel-order', {
      id,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return true;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}

export async function AddOrder({ stateId, UserAddress_Id, quantity, total, comment, servicesOrders, transport, quantityS }) {
  try {
    const res = await axiosUrl.post('order/add', {
      stateId,
      UserAddress_Id,
      quantity,
      total,
      comment,
      userId: "string",
      servicesOrders,
      transport,
      quantityS,
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',

      },
    });
    if (res.status == 200) {

      return true;
    }

    return false;
  }
  catch (e) {
    logger.error(e);
    return [];
  }
}
