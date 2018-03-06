// Ответ ajax - должен быть согласован с enum JsonResponseState
ajaxState = { Ok: 0, Error: -1 };
$(function (section) {
	section.ProductViewModel = function (viewModel) {
		//var model = this;
		var model = ko.mapping.fromJS(viewModel);
		section.baseModel.apply(model);

		model.SelectedItemS = ko.observable(0);

		model.create = function () {

			if (model.Errors().length > 0) {
				model.Errors.showAllMessages();
				model.ShowSummaryHints(true);

				return;
			}

			$.ajax({
				url: section.urls.ValidateContact,
				type: 'POST',
				dataType: 'json',
				contentType: "application/json; charset=utf-8",
				data: ko.mapping.toJSON(model), //Seller.Ready 2030
				success: function (response) {

					if (response.State === ajaxState.Ok) {
						alert('ok');
					} else {
						//Выводим сообщения об ошибках
						if (response.Data != null) {
							model.SetUpAllServerErrors(response.Data);
						}
					}
				},
				complete: function () {
				}
			});	
		};


		model.SelectedItemS.subscribe(function (data) {
			model.SelectedItem(data);
		});

		return model;
	};

	// Создание экземпляра модели
	section.ProductViewModel = new section.ProductViewModel(viewModel);

	//ko.validation.registerExtenders();
	ko.applyBindings(section.ProductViewModel, $("#root").get(0));//TODO: изменить на 'ko-settings'. сейчас только для css селекторов используется этот id

}(product));