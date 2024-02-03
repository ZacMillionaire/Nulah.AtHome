import { type Invalidator, type Subscriber, type Unsubscriber, writable } from 'svelte/store';
import { type BasicEvent } from './models/BasicEvent';
import type { NewBasicEventRequest } from './models/NewBasicEventRequest';
import { dev } from '$app/environment';

export const EventStore: IEventStore = CreateEventStore();

function CreateEventStore(): IEventStore {
	const { subscribe, set, update } = writable<BasicEvent[]>([]);

	GetEvents()
		.then(x => set(x));

	return {
		subscribe,
		add: AddNewEvent,
		GetEvents: GetEvents,
		CreateEvent: CreateEvent
	};

	function AddNewEvent(newEvent: BasicEvent) {
		// Ensure that we have proper data types for this event as it
		// might be from a JSON response, and for those cases dates may instead
		// be a string and not a real date object.
		// This is mostly likely because I'm just raw fetching without a library or anything
		const event = {
			Start: new Date(newEvent.Start),
			End: newEvent.End ? new Date(newEvent.End) : null,
			Tags: newEvent.Tags,
			Id: newEvent.Id,
			Description: newEvent.Description,
			Version: newEvent.Version
		} as BasicEvent;

		return update(u => ([...u, event]));
	}

	async function GetEvents(): Promise<BasicEvent[]> {
		// if (dev) {
		// 	return [{
		// 		Description: 'test',
		// 		Start: new Date(),
		// 		Id: 0,
		// 		Tags: null,
		// 		End: null
		// 	}];
		// }

		return await fetch('https://localhost:7150/api/v1/Events/Get')
			.then(async (x: Response) => (await x.json() as BasicEvent[])
				.map((x: BasicEvent) => {
						// parse each "BasicEvent" we received into a proper BasicEvent
						// with correct Date objects.
						// Otherwise Start/End will be a string representation that we received from the endpoint
						return {
							Start: new Date(x.Start),
							End: x.End ? new Date(x.End) : null,
							Tags: x.Tags,
							Id: x.Id,
							Description: x.Description,
							Version: x.Version
						} as BasicEvent;
					}
				) as BasicEvent[])
			.catch(error => {
				console.log(error);
				return [];
			});
	}

	async function CreateEvent(newEventDescription: NewBasicEventRequest) {

		// if (dev) {
		// 	AddNewEvent({
		// 		Description: newEventDescription.Description,
		// 		Start: newEventDescription.Start,
		// 		End: newEventDescription.End,
		// 		Id: 0,
		// 		Tags: newEventDescription.Tags
		// 	});
		// 	return;
		// }


		const headers: Headers = new Headers();
		headers.set('Content-Type', 'application/json');
		// We also need to set the `Accept` header to `application/json`
		// to tell the server that we expect JSON in response
		headers.set('Accept', 'application/json');

		const request: RequestInfo = new Request('https://localhost:7150/api/v1/Events/Create', {
			// We need to set the `method` to `POST` and assign the headers
			method: 'POST',
			headers: headers,
			// Convert the user object to JSON and pass it as the body
			body: JSON.stringify(newEventDescription)
		});

		// Send the request and print the response
		// TODO: create a strong type for this so it returns something other than Response
		const createResult = await fetch(request)
			.then(async res => {
				if (res.ok) {
					return await res.json() as BasicEvent;
				}
				throw new Error(JSON.stringify(await res.json()));
			}, rejected => {
				console.error(rejected);
			});

		if (createResult !== null) {
			AddNewEvent(createResult as BasicEvent);
		}
	}
}

export interface IEventStore {
	add: (newEvent: BasicEvent) => void;
	subscribe: (this: void, run: Subscriber<BasicEvent[]>, invalidate?: Invalidator<BasicEvent[]>) => Unsubscriber;
	GetEvents: () => Promise<BasicEvent[]>;
	CreateEvent: (newEventDescription: NewBasicEventRequest) => Promise<void | BasicEvent | Error>;
}