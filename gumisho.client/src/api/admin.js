import axiosUrl from "@/config/axiosApp";
import { logger } from "@/utils/logger";

export async function GetDriverInfo(driverId, days, from, to) {
  try {
    const res = await axiosUrl.get(`admin/drivers?driverId=${driverId}&days=${days}&from=${from}&to=${to}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetAllOrders(days, from, to, status, stateId) {
  try {
    if (!days) {
      days = 0;
    }
    if (!status) {
      status = 6;
    }
    if (!stateId) {
      stateId = 0;
    }
    const res = await axiosUrl.get(`admin/orders?status=${status}&days=${days}&from=${from}&to=${to}&stateId=${stateId}`);
 
    return res.data;
   
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetDriverOrders(driverId, days, from, to, status) {
  try {
    const res = await axiosUrl.get(`admin/drivers?driverId=${driverId}&days=${days}&from=${from}&to=${to}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetAllUsers() {
  try {
    const res = await axiosUrl.get('admin/users');
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function GetAllRoles() {
  try {
    const res = await axiosUrl.get('admin/roles');
    return res.data;

  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function UsersInRole(role) {
  try {
    const res = await axiosUrl.get(`admin/user-roles?role=${role}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function addRole({ username, roleName }) {
  try {
    const res = await axiosUrl.post(`admin/add-role?roleName=${roleName}&username=${username}`, {
     
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return true;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function removeRole({ username, roleName }) {
  try {
    const res = await axiosUrl.post(`Admin/remove-role?username=${username}&roleName=${roleName}`, {
      
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return true;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function setLocation({ stateId, username }) {
  try {
    const res = await axiosUrl.post(`admin/set-location?stateId=${stateId}&username=${username}`, {
     
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return true;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}
export async function setWorkPlace({ stateId, home, drive }) {
  try {
    const res = await axiosUrl.post(`Admin/driver-or-home?stateId=${stateId}&home=${home}&drive=${drive}`, {

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
     
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function RemoveDriverLocation({ stateId, userId }) {
  try {
    const res = await axiosUrl.post(`Admin/remove-driver-location?stateId=${stateId}&userId=${userId}`, {

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function AddnewLocation({ stateName }) {
  try {
    const res = await axiosUrl.post(`Admin/new-location?stateName=${stateName}`, {

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function DriverInState(stateId) {
  try {
    const res = await axiosUrl.get(`Admin/driver-in-state?stateId=${stateId}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}
export async function AllStates() {
  try {
    const res = await axiosUrl.get(`Admin/all-states`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function DeleteState(id) {
  try {
    const res = await axiosUrl.get(`Admin/delete-state?id=${id}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}
export async function EditState({ id, name, drive, home }) {
  try {
    const res = await axiosUrl.post(`Admin/edit-location`, {
      id, name, drive, home
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function AllServices() {
  try {
    const res = await axiosUrl.get(`Service/all`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}
export async function ShowService(id) {
  try {
    const res = await axiosUrl.get(`Service/show?id=${id}`);
    return res.data;
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export async function AddService({ name, description, quantity, priceCarpet, priceDelivery }) {
  try {
    const res = await axiosUrl.post(`Service/add`, {
      name, description, quantity, priceCarpet, priceDelivery
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function EditService({ id, name, description, quantity, priceCarpet, priceDelivery }) {
  try {
    const res = await axiosUrl.post(`Service/edit`, {
      id, name, description, quantity, priceCarpet, priceDelivery 
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}

export async function DeleteService(id) {
  try {
    const res = await axiosUrl.post(`Service/delete?id=${id}`, {
      
    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}
export async function ChangeOrderStatus(orderId,status) {
  try {
    const res = await axiosUrl.post(`Admin/order-status?orderId=${orderId}&status=${status}`, {

    }, {
      headers: {
        'accept': 'application/json',
        'Content-Type': 'application/json',
      },
    });

    if (res.status === 200) {
      return res.data;
    }

    return false;
  } catch (e) {
    logger.error(e);
    return [];
  }
}
