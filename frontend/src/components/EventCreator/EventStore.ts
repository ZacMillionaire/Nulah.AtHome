import { type Invalidator, type Subscriber, type Unsubscriber, writable } from 'svelte/store';
import type { BasicEvent } from 'models/BasicEvent';

export const EventStore: IEventStore = CreateEventStore();

function CreateEventStore(): IEventStore {
	const { subscribe, set, update } = writable<BasicEvent[]>([]);

	set([{
		Name: 'Test event 1',
		Id: 1,
		Tags: [],
		Start: new Date()
	}, {
		Name: 'Test event 2',
		Id: 2,
		Tags: [],
		Start: new Date(),
		End: new Date()
	}]);

	return {
		subscribe,
		add: AddNewEvent
	};

	function AddNewEvent(newEvent: BasicEvent) {
		return update(u => ([...u, newEvent]));
	}
}

export interface IEventStore {
	add: (newEvent: BasicEvent) => void;
	subscribe: (this: void, run: Subscriber<BasicEvent[]>, invalidate?: Invalidator<BasicEvent[]>) => Unsubscriber;
}