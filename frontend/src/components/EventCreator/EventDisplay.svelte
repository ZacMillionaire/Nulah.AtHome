<script lang="ts">
	import type { BasicEvent } from './models/BasicEvent';
	import EventCreator from './EventCreator.svelte';
	import type { EventRequestFormModel } from './models/EventRequestFormModel';
	import { EventStore } from './EventStore';

	export let Event: BasicEvent;
	let editMode: boolean = false;

	function formatDateToString(inputDate?: Date | null): string | null {
		if (!inputDate) {
			return null;
		}

		// Create a new date from the given date to avoid mutations
		let d = new Date(inputDate);
		// adjust it to local time for the ISOString
		d.setMinutes(d.getMinutes() - d.getTimezoneOffset());

		return d.toISOString().slice(0, 16);
	}

	let formData: EventRequestFormModel = {
		Id : Event.Id,
		Version : Event.Version,
		Description: Event.Description,
		Start: formatDateToString(Event.Start)!,
		End: formatDateToString(Event.End),
		Tags: Event.Tags?.join(', ') ?? null
	};

</script>

<style lang="scss">
  .event {
    border: 1px solid #c2c2c2;
    border-radius: 5px;
    padding: 5px;
    margin: 5px;
    min-width: var(--column-width);
  }

  h1 {
    padding: 0;
    margin: 0;
  }
</style>


<div class="event">
	<h1>{Event.Description}</h1>
	<button on:click={() => (editMode = true)}>edit</button>
	<div>Start: {Event.Start.toLocaleString()}</div>
	{#if Event.End}
		<div>End: {Event.End?.toLocaleString()}</div>
	{/if}
	{#if editMode}
		<EventCreator formData="{formData}" formSubmitAction="{EventStore.UpdateEvent}" />
	{/if}
</div>