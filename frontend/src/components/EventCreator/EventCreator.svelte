<script lang="ts">
	import { EventStore } from './EventStore';

	function createEvent(e: SubmitEvent) {
		let eventFormData = new FormData(e.target as HTMLFormElement);

		EventStore.add({
			Name: eventFormData.get('name') as string,
			Id: 3,
			Tags: [],
			Start: new Date(),
			End: new Date()
		});
	}
</script>

<form on:submit|preventDefault={createEvent}>
	<input name="name" id="name"/>
	<button>submit</button>
</form>