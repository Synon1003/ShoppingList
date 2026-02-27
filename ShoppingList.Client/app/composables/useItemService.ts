import axios from 'axios'

type Item = {
  id: string
  name: string
  description: string
  price: number
  updatedAt: string
  status: string | number
}

type CreateItemDto = {
  name: string
  description: string
  price: number
}


const statusDeserializerDict: Record<number, string> = {
  0: 'Not Purchased',
  1: 'Purchased',
}

const mockApiUrl = import.meta.env.VITE_API_URL || 'https://localhost:8080'

function deserializeItemDto(data: { id: string; name: string; description: string; price: number; updatedAt: string; status: number | string }): Item {
  return {
    id: data.id,
    name: data.name,
    description: data.description,
    price: data.price,
    updatedAt: data.updatedAt,
    status: statusDeserializerDict[(data.status as number) ?? -1] || data.status || '',
  }
}

function serializeCreateItemDto({ name, description, price }: CreateItemDto): CreateItemDto {
  return {
    name: name,
    description: description,
    price: price,
  }
}

async function fetchItems(): Promise<[Item[], string]> {
  try {
    const response = await axios.get(mockApiUrl + '/items/all')
    const items: Item[] = []
    for (let idx = 0; idx < response.data.length; idx++) items.push(deserializeItemDto(response.data[idx]))

    return [items, '']
  } catch (error: any) {
    return Promise.resolve([[], 'Error. The shopping list items are not reachable.'])
  }
}

async function fetchItemById(id: string): Promise<[Item | null, string]> {
  try {
    const response = await axios.get(mockApiUrl + '/items/' + id)
    return Promise.resolve([deserializeItemDto(response.data), ''])
  } catch (error: any) {
    return Promise.resolve([null, 'Error. The item is not reachable.'])
  }
}

async function createItem(item: CreateItemDto): Promise<[boolean, string]> {
  try {
    await axios.post(mockApiUrl + '/items', serializeCreateItemDto(item))
    return Promise.resolve([true, "The item was added successfully."])
  } catch (error: any) {
    if (axios.isAxiosError(error)) {
      const status = error.response?.status

      if (status === 400) return Promise.resolve([false, "Error. Validation failed."])

      return Promise.resolve([false, "Server error. The item was not added to the shopping list."])
    }

    return Promise.resolve([false, 'Error. The item was not added to the shopping list.'])
  }
}

async function patchItem(id: string): Promise<[boolean, string]> {
  try {
    await axios.patch(mockApiUrl + '/items/' + id)
    return Promise.resolve([true, "The status of the item changed."])
  } catch (error: any) {
    return Promise.resolve([false, 'Error. The status of the item did not change.'])
  }
}

async function deleteItem(id: string): Promise<[boolean, string]> {
  try {
    await axios.delete(mockApiUrl + '/items/' + id)
    return Promise.resolve([true, "The item was deleted."])
  } catch (error: any) {
    return Promise.resolve([false, 'Error. Could not delete the item.'])
  }
}

export const itemService = {
  fetchItems,
  fetchItemById,
  createItem,
  patchItem,
  deleteItem,
}