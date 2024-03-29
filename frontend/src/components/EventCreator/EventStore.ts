import { type Invalidator, type Subscriber, type Unsubscriber, writable } from 'svelte/store';
import { type BasicEvent } from './models/BasicEvent';
import type { NewBasicEventRequest } from './models/NewBasicEventRequest';
import type { UpdateBasicEventRequest } from './models/UpdateBasicEventRequest';
import type { EventRequestFormModel } from './models/EventRequestFormModel';
import config from '../../config';

export const EventStore: IEventStore = CreateEventStore();

function CreateEventStore(): IEventStore {
	const { subscribe, set, update } = writable<BasicEvent[]>([]);

	GetEvents()
		.then(x => set(x));

	return {
		subscribe,
		add: AddNewEvent,
		GetEvents: GetEvents,
		CreateEvent: CreateEvent,
		UpdateEvent: UpdateEvent
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
		return await fetch(`${config.backendDomain}/api/v1/Events/Get`)
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

	async function CreateEvent(newEventDescription: EventRequestFormModel) {

		const newEvent = {
			Description: newEventDescription.Description,
			Start: new Date(newEventDescription.Start),
			// Make sure that if a user has cleared a form value that we correctly get a null
			// instead of the empty string it will receive.
			// We don't need to do it for Start so long as it is tagged as required in the markup.
			End: newEventDescription.End ? new Date(newEventDescription.End) : null,
			Tags: newEventDescription.Tags?.split(',').map(x => x.trim())
		} as NewBasicEventRequest;

		const createResult = await PostRequestAsync<NewBasicEventRequest, BasicEvent>(
			`${config.backendDomain}/api/v1/Events/Create`,
			newEvent
		);

		if (createResult !== null) {
			AddNewEvent(createResult as BasicEvent);
		}

		return createResult;
	}

	async function PostRequestAsync<TReq, TRes>(requestUri: string, requestObject: TReq): Promise<void | TRes> {

		const headers: Headers = new Headers();
		headers.set('Content-Type', 'application/json');
		// We also need to set the `Accept` header to `application/json`
		// to tell the server that we expect JSON in response
		headers.set('Accept', 'application/json');

		const request: RequestInfo = new Request(requestUri, {
			// We need to set the `method` to `POST` and assign the headers
			method: 'POST',
			headers: headers,
			// Convert the user object to JSON and pass it as the body
			body: JSON.stringify(requestObject)
		});

		const requestResult = fetch(request)
			.then(async res => {
				if (res.ok) {
					return await res.json() as TRes;
				}
				throw new Error(JSON.stringify(await res.json()));
			});

		return requestResult;
	}

	async function UpdateEvent(updateBasicEventRequest: EventRequestFormModel) {

		const updatedEvent = {
			Id: updateBasicEventRequest.Id,
			Version: updateBasicEventRequest.Version,
			Description: updateBasicEventRequest.Description,
			Start: new Date(updateBasicEventRequest.Start),
			// Make sure that if a user has cleared a form value that we correctly get a null
			// instead of the empty string it will receive.
			// We don't need to do it for Start so long as it is tagged as required in the markup.
			End: updateBasicEventRequest.End ? new Date(updateBasicEventRequest.End) : null,
			Tags: updateBasicEventRequest.Tags?.split(',').map(x => x.trim())
		} as UpdateBasicEventRequest;

		const updatedResult = await PostRequestAsync<UpdateBasicEventRequest, BasicEvent>(
			`${config.backendDomain}/api/v1/Events/Update`,
			updatedEvent
		);

		if (updatedResult) {
			update((u: BasicEvent[]) => {
				// This feels like a dumb way to update the item in an array?
				const updateTarget = u.find(x => x.Id === updatedEvent.Id);

				if (updateTarget) {
					const idx = u.indexOf(updateTarget);
					u[idx] = updatedResult;
				}

				return u;
			});
		}

		return updatedResult;
	}
}

export interface IEventStore {
	add: (newEvent: BasicEvent) => void;
	subscribe: (this: void, run: Subscriber<BasicEvent[]>, invalidate?: Invalidator<BasicEvent[]>) => Unsubscriber;
	GetEvents: () => Promise<BasicEvent[]>;
	CreateEvent: (newEventDescription: EventRequestFormModel) => Promise<void | BasicEvent | Error>;
	UpdateEvent: (newEventDescription: EventRequestFormModel) => Promise<void | BasicEvent | Error>;
}